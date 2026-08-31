using Contracts.Common.Messages;
using Contracts.Enums;
using Contracts.Requests.ContentRequests;
using Contracts.Requests.RequestParameters;
using Contracts.Responses;
using Contracts.Responses.ContentResponses;
using Contracts.Results;
using Domain.Entities.Content;
using Domain.Interfaces.ContentInterfaces;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Domain.Interfaces.ImageInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniNet.Extensions;
using UniNet.Models.ContentModels;

namespace UniNet.Controllers.ContentControllers
{
    /// <summary>
    /// متحكّم واحد للمحتوى بنوعيه — لا PostController و AnnouncementController.
    /// Post و Announcement صنفان فارغان متطابقان، والانقسام كان سيُنتج متحكّمين متطابقين.
    /// النمط نفسه المتّبع في EmployeeController: قراءة مشتركة، وكتابة بمسار فرعي لكل نوع
    /// يحمل قائمة أدواره الخاصة.
    ///
    /// لا يوجد ImageController: الصورة مورد تابع لعنصر محتوى ولا وجود لها بدونه،
    /// فعنوانها تحت مالكها. والعنونة بـ contentItemId لا imageId: العلاقة 1:1 إلزامية
    /// فمعرّف المحتوى يحدّد الصورة تحديدًا تامًّا، ويختفي imageId من واجهة الـ API كليًّا.
    ///
    /// معرّفات المسار هنا لا [FromQuery] IdParameter كبقية المتحكّمات: مسار الصورة الفرعي
    /// يفرض المعرّف في المسار، وخلط الأسلوبين داخل متحكّم واحد أسوأ من مخالفة الاصطلاح فيه.
    ///
    /// ملاحظة للعميل: نقطة الصورة محميّة بـ Bearer، والمتصفح لا يُرفق ترويسة Authorization
    /// مع img src — فتظهر الصورة مكسورة دائمًا. الطريقة الصحيحة من الواجهة:
    ///     const res = await fetch(url, { headers: { Authorization: `Bearer ${token}` } });
    ///     img.src = URL.createObjectURL(await res.blob());
    /// ولا يُستبدل هذا بوسيط ?token= في الرابط: يُسجَّل التوكن عندها في تاريخ المتصفح
    /// وترويسات Referer وسجلات الخادم، ولكل نقاط النظام لا لهذه وحدها.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        private const string ContentManagerRoles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin,BatchAdmin";
        private const int MaxUploadBytes = 6 * 1024 * 1024;

        private readonly IContentService _contentService;
        private readonly IImageService _imageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<ContentController> _logger;

        public ContentController(IContentService contentService, IImageService imageService,
            ICurrentUserService currentUserService, IAuthorizationService authorizationService,
            ILogger<ContentController> logger)
        {
            _contentService = contentService;
            _imageService = imageService;
            _currentUserService = currentUserService;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        // -------------------------------------------------------------- reads

        /// <summary>خلاصة المستخدم: ما يقع هو داخل جمهوره. بلا قائمة أدوار — كل مُصادَق عليه له خلاصة.</summary>
        [Authorize]
        [HttpGet("feed", Name = "GetContentFeed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<ContentFeedItemDTO>>> GetContentFeed(
            [FromQuery] ContentFeedFilterDTO? filter, [FromQuery] PagedResultParameters @pagedResultParameters)
        {
            var feed = await _contentService.GetFeed(_currentUserService.ToUserScope(), filter,
                pagedResultParameters.PageNumber, pagedResultParameters.PageSize);

            return feed.ToPagedActioneResult();
        }

        /// <summary>قائمة الإدارة: ما يستطيع هذا المسؤول تعديله وحذفه — سؤال معاكس للخلاصة.</summary>
        [Authorize(Roles = ContentManagerRoles)]
        [HttpGet(Name = "GetManagedContent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<ContentItemDTO>>> GetManagedContent(
            [FromQuery] ContentFilterDTO? filter, [FromQuery] PagedResultParameters @pagedResultParameters)
        {
            var content = await _contentService.GetManagedContent(_currentUserService.ToUserScope(), filter,
                _currentUserService.UserId, pagedResultParameters.PageNumber, pagedResultParameters.PageSize);

            return content.ToPagedActioneResult();
        }

        [Authorize]
        [HttpGet("{contentItemId:int}", Name = "GetContentItemById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetaieldContentItemDTO>> GetContentItemById(int contentItemId)
        {
            var viewInfo = await _contentService.GetContentViewInfoAsync(contentItemId);
            if (viewInfo == null)
                return NotFound(ErrorMessages.NotFound<ContentItem>(contentItemId));

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, viewInfo, "ContentViewPolicy");
            if (!authorizationResult.Succeeded)
                return authorizationResult.NotAuthorized();

            var content = await _contentService.GetDetaieldContentItemDTOById(contentItemId);
            return content.GetResourceEndpoints(contentItemId, typeof(ContentItem).Name);
        }

        [Authorize]
        [HttpGet("{contentItemId:int}/image", Name = "GetContentImage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetContentImage(int contentItemId)
        {
            var imageInfo = await _imageService.GetFileInfoByContentItemIdAsync(contentItemId);
            if (imageInfo == null)
                return NotFound();

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, imageInfo.ViewInfo, "ContentViewPolicy");
            if (!authorizationResult.Succeeded)
                return authorizationResult.NotAuthorized();

            var absolutePath = _imageService.GetAbsolutePath(imageInfo.RelativePath);

            // صف موجود وملف مفقود = بيانات غير متسقة، لا غياب صلاحية. 404 بلا كشف السبب.
            // System.IO.File صراحةً لتفادي التصادم مع ControllerBase.File
            if (!System.IO.File.Exists(absolutePath))
            {
                // يُسجَّل ولا يُكشف: الردّ يبقى 404 مطابقًا لردّ منع الصلاحية كي لا يُسرَّب
                // وجود المورد، لكن بلا هذا السطر يصير استرجاعُ قاعدةٍ على جهاز فارغ
                // App_Data صامتًا تمامًا — صفوف سليمة وصور مفقودة بلا أي أثر يُميّزها.
                _logger.LogWarning("Image row {ImageId} for content {ContentItemId} points at a missing file: {RelativePath}",
                    imageInfo.ImageId, contentItemId, imageInfo.RelativePath);

                return NotFound();
            }

            // private لا public: المحتوى مُفوَّض، فلا يجوز لأي وسيط مشترك تخزينه
            Response.Headers.CacheControl = "private, max-age=86400";

            // النقطة الوحيدة في التطبيق التي تُعيد بايتات رفعها مستخدم إلى المتصفح
            // تحت أصل التطبيق نفسه. بلا nosniff يحقّ للمتصفح تجاهل ContentType المعلن
            // ويستنتج نوعًا آخر — و ContentType مخزَّن كما أرسله العميل، بينما فحص
            // التوقيع يقارن البايتات بالامتداد لا بالترويسة، فالاثنان غير متقاطعَين.
            Response.Headers["X-Content-Type-Options"] = "nosniff";

            // PhysicalFile يستعمل SendFileAsync فلا يُحمَّل الملف في الذاكرة إطلاقًا
            return PhysicalFile(absolutePath, imageInfo.ContentType, enableRangeProcessing: true);
        }

        // -------------------------------------------------------------- writes

        [Authorize(Roles = ContentManagerRoles)]
        [HttpPost("post", Name = "AddPost")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(MaxUploadBytes)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
        public async Task<ActionResult<AddUpdateServiceResponse<DetaieldContentItemDTO>>> AddPost(
            [FromForm] AddContentForm form)
        {
            return await AddContent(form, EnContentType.Post);
        }

        [Authorize(Roles = ContentManagerRoles)]
        [HttpPost("announcement", Name = "AddAnnouncement")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(MaxUploadBytes)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
        public async Task<ActionResult<AddUpdateServiceResponse<DetaieldContentItemDTO>>> AddAnnouncement(
            [FromForm] AddContentForm form)
        {
            return await AddContent(form, EnContentType.Announcement);
        }

        [Authorize(Roles = ContentManagerRoles)]
        [HttpPut("{contentItemId:int}", Name = "UpdateContent")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(MaxUploadBytes)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AddUpdateServiceResponse<DetaieldContentItemDTO>>> UpdateContent(
            int contentItemId, [FromForm] UpdateContentForm form)
        {
            var manageInfo = await _contentService.GetContentManageInfoAsync(contentItemId);
            if (manageInfo == null)
                return NotFound(ErrorMessages.NotFound<ContentItem>(contentItemId));

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, manageInfo, "ContentManagePolicy");
            if (!authorizationResult.Succeeded)
                return authorizationResult.NotAuthorized();

            var response = await _contentService.UpdateContent(_currentUserService.ToUserScope(), form.ToDTO(),
                form.Image.ToUploadedFile(), contentItemId, _currentUserService.UserId);

            return response.ToActionResult();
        }

        [Authorize(Roles = ContentManagerRoles)]
        [HttpDelete("{contentItemId:int}", Name = "DeleteContent")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteContent(int contentItemId)
        {
            var manageInfo = await _contentService.GetContentManageInfoAsync(contentItemId);
            if (manageInfo == null)
                return NotFound(ErrorMessages.NotFound<ContentItem>(contentItemId));

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, manageInfo, "ContentManagePolicy");
            if (!authorizationResult.Succeeded)
                return authorizationResult.NotAuthorized();

            var result = await _contentService.Delete(contentItemId);
            return result.ToDeleteActionResult<ContentItem>(contentItemId);
        }

        // -------------------------------------------------------------- shared

        /// <summary>
        /// النوع يأتي من المسار لا من جسم الطلب، فلا يستطيع عميلٌ يملك حق نشر المنشورات
        /// أن ينشر إعلانًا بتغيير حقل في النموذج.
        /// </summary>
        private async Task<ActionResult<AddUpdateServiceResponse<DetaieldContentItemDTO>>> AddContent(
            AddContentForm form, EnContentType type)
        {
            var response = await _contentService.AddContent(_currentUserService.ToUserScope(), form.ToDTO(),
                form.Image.ToUploadedFile(), type, _currentUserService.UserId);

            return response.ToActionResult();
        }
    }
}
