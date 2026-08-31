using Domain.Interfaces.ImageInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniNet.Extensions;

namespace UniNet.Controllers.ContentControllers
{
    /// <summary>
    /// المتحكّم الخاص بالمحتوى. لا يوجد ImageController — الصورة مورد تابع لعنصر محتوى
    /// ولا وجود لها بدونه، فعنوانها الطبيعي هو تحت مالكها: api/content/{id}/image
    ///
    /// العنونة بـ contentItemId لا بـ imageId مقصودة:
    ///   • العلاقة 1:1 إلزامية، فمعرّف المحتوى يحدّد الصورة تحديدًا تامًّا.
    ///   • العميل يملك معرّف المنشور أصلًا وهو يعرضه — فلا يحتاج معرّفًا ثانيًا.
    ///   • التفويض على المحتوى مباشرةً بلا خطوة وسيطة لاكتشاف المالك.
    ///   • imageId يختفي من واجهة الـ API كليًّا: تفصيلة تخزين لا يحتاج أحد لمعرفتها.
    ///
    /// نقاط Post و Announcement تُضاف إلى هذا المتحكّم لاحقًا.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IAuthorizationService _authorizationService;

        public ContentController(IImageService imageService, IAuthorizationService authorizationService)
        {
            _imageService = imageService;
            _authorizationService = authorizationService;
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

            // صف موجود وملف مفقود = بيانات غير متسقة، لا "غير مصرّح". نُرجع 404 ولا نكشف السبب.
            // System.IO.File صراحةً لتفادي التصادم مع ControllerBase.File
            if (!System.IO.File.Exists(absolutePath))
                return NotFound();

            // private لا public: المحتوى مُفوَّض، فلا يجوز لأي وسيط مشترك (Proxy/CDN) تخزينه.
            Response.Headers.CacheControl = "private, max-age=86400";

            // PhysicalFile يستعمل SendFileAsync فلا يُحمَّل الملف في الذاكرة إطلاقًا.
            return PhysicalFile(absolutePath, imageInfo.ContentType, enableRangeProcessing: true);
        }
    }
}
