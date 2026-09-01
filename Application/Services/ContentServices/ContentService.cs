using Contracts.Common.AuthorizationInfos;
using Contracts.Common.AuthorizationInfos.ContentAuthorizationInfo;
using Contracts.Common.Extensions;
using Contracts.Common.Mappers;
using Contracts.Enums;
using Contracts.Requests.ContentRequests;
using Contracts.Requests.ImageRequests;
using Contracts.Requests.RequestParameters;
using Contracts.Responses;
using Contracts.Responses.ContentResponses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using Domain.Entities.Content;
using Domain.Entities.Images;
using Domain.Interfaces.ContentInterfaces;
using Domain.Interfaces.ImageInterfaces;
using Domain.Interfaces.UnitOfWork;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.ContentServices
{
    public class ContentService : IContentService
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly IImageService _imageService;
        private readonly IValidator<AddContentDTO> _addValidator;
        private readonly IValidator<UpdateContentDTO> _updateValidator;

        public ContentService(IUnitOfWorkRepository unitOfWorkRepository, IImageService imageService,
            IValidator<AddContentDTO> addValidator, IValidator<UpdateContentDTO> updateValidator)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
            _imageService = imageService;
            _addValidator = addValidator;
            _updateValidator = updateValidator;
        }

        // ------------------------------------------------------------------ reads

        public async Task<PagedResult<ContentFeedItemDTO>> GetFeed(UserScope? viewer, ContentFeedFilterDTO? filter,
            int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.ContentRepository.GetFeed(viewer, filter, pageNumber, pageSize);
        }

        public async Task<PagedResult<ContentItemDTO>> GetManagedContent(UserScope? actor, ContentFilterDTO? filter,
            int currentUserId, int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.ContentRepository.GetManagedContent(actor, filter, currentUserId,
                pageNumber, pageSize);
        }

        public async Task<DetaieldContentItemDTO?> GetDetaieldContentItemDTOById(int contentItemId)
        {
            return await _unitOfWorkRepository.ContentRepository.GetDetaieldContentItemDTOById(contentItemId);
        }

        public async Task<ContentViewInfo?> GetContentViewInfoAsync(int contentItemId)
        {
            return await _unitOfWorkRepository.ContentRepository.GetContentViewInfoAsync(contentItemId);
        }

        public async Task<ContentManageInfo?> GetContentManageInfoAsync(int contentItemId)
        {
            return await _unitOfWorkRepository.ContentRepository.GetContentManageInfoAsync(contentItemId);
        }

        public async Task<bool> IsExistsById(int contentItemId)
        {
            return await _unitOfWorkRepository.ContentRepository.IsExistsById(contentItemId);
        }

        // ------------------------------------------------------------------ create

        public async Task<AddUpdateServiceResponse<DetaieldContentItemDTO>> AddContent(UserScope? scope,
            AddContentDTO newContent, UploadedFileDTO? file, EnContentType type, int currentUserId)
        {
            var validationResult = await _addValidator.ValidateAsync(newContent);
            if (!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<DetaieldContentItemDTO>.Failure(
                    validationResult.Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(),
                    EnErrorTypes.InvalidData);
            }

            var targetResult = await ResolveScopeTargetAsync(scope, newContent.Scope, newContent.TargetId);
            if (!targetResult.IsSuccess)
            {
                return AddUpdateServiceResponse<DetaieldContentItemDTO>.Failure(
                    targetResult.Errors!, targetResult.ErrorType!.Value);
            }

            var target = targetResult.Data!;

            if (!ContentScopeExtension.IsScopeConsistent(newContent.Scope, target.UniversityId, target.CollegeId,
                    target.DepartmentId, target.BatchId))
            {
                // شبكة أمان ثانية: القيد في قاعدة البيانات يحرس الجدول، وهذا يحرس المستخدم
                // من رسالة قيدٍ غامضة — ويمنع كتابة الملف على القرص من أجل صفٍّ سيُرفَض.
                return AddUpdateServiceResponse<DetaieldContentItemDTO>.Failure(
                    new List<string> { "The resolved scope targets are inconsistent with the requested scope." },
                    EnErrorTypes.InvalidData);
            }

            // النوع يُحدَّد بالصنف المُنشأ لا بإسناد Type: العمود هو مميّز TPH ويكتبه EF بنفسه.
            // إسناده يدويًا بقيمة مخالفة للصنف لا يُصحَّح — فيُدرَج Post موسومًا كإعلان.
            ContentItem contentItem = type == EnContentType.Announcement ? new Announcement() : new Post();

            contentItem.Title = newContent.Title;
            contentItem.Body = newContent.Body;
            contentItem.Scope = newContent.Scope;
            contentItem.UniversityId = target.UniversityId;
            contentItem.CollegeId = target.CollegeId;
            contentItem.DepartmentId = target.DepartmentId;
            contentItem.BatchId = target.BatchId;
            contentItem.CreatedAt = DateTime.UtcNow;
            contentItem.CreatedByUserId = currentUserId;

            Image? preparedImage = null;

            if (file != null)
            {
                var imageResult = await _imageService.PrepareAsync(file, currentUserId);
                if (!imageResult.IsSuccess)
                {
                    return AddUpdateServiceResponse<DetaieldContentItemDTO>.Failure(
                        imageResult.Errors!, imageResult.ErrorType!.Value);
                }

                preparedImage = imageResult.Data!;
                // إسناد الملاحية يكفي: EF يُدرج الصفّين في SaveChanges واحد ويملأ المفتاح الأجنبي.
                contentItem.Image = preparedImage;
            }

            try
            {
                _unitOfWorkRepository.ContentRepository.Add(contentItem);
                await _unitOfWorkRepository.CompleteAsync();
            }
            catch
            {
                // تعويض: الملف كُتب قبل أن تُلمس قاعدة البيانات، فسقوطها يتركه يتيمًا.
                // الترتيب مقصود — وضع الفشل الوحيد الممكن مساحةُ قرصٍ ضائعة لا صورةٌ مكسورة.
                if (preparedImage != null)
                {
                    _imageService.DeletePhysicalFile(preparedImage.RelativePath);
                }

                throw;
            }

            var dto = await GetDetaieldContentItemDTOById(contentItem.ContentItemId);
            return AddUpdateServiceResponse<DetaieldContentItemDTO>.Success(dto!);
        }

        // ------------------------------------------------------------------ update

        public async Task<AddUpdateServiceResponse<DetaieldContentItemDTO>> UpdateContent(UserScope? scope,
            UpdateContentDTO updatedContent, UploadedFileDTO? file, int contentItemId, int currentUserId)
        {
            var validationResult = await _updateValidator.ValidateAsync(updatedContent);
            if (!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<DetaieldContentItemDTO>.Failure(
                    validationResult.Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(),
                    EnErrorTypes.InvalidData);
            }

            var manageInfo = await GetContentManageInfoAsync(contentItemId);
            if (manageInfo == null)
            {
                return AddUpdateServiceResponse<DetaieldContentItemDTO>.ResourceDoesntExist<ContentItem>();
            }

            // 404 لا 403 — نمط المشروع في عدم تسريب وجود المورد
            if (!scope.CanManageContent(manageInfo, currentUserId))
            {
                return AddUpdateServiceResponse<DetaieldContentItemDTO>.ResourceDoesntExist<ContentItem>();
            }

            var contentItem = await _unitOfWorkRepository.ContentRepository.GetEntityById(contentItemId);

            contentItem!.Title = updatedContent.Title;
            contentItem.Body = updatedContent.Body;
            contentItem.UpdatedAt = DateTime.UtcNow;
            contentItem.UpdatedByUserId = currentUserId;

            // النطاق لا يُمَس هنا: UpdateContentDTO لا يحمله أصلًا (انظر تعليقه).

            var imageResult = await ApplyImageChangeAsync(contentItemId, contentItem, file,
                updatedContent.RemoveImage, currentUserId);

            if (!imageResult.IsSuccess)
            {
                return AddUpdateServiceResponse<DetaieldContentItemDTO>.Failure(
                    imageResult.Errors!, imageResult.ErrorType!.Value);
            }

            var dto = await GetDetaieldContentItemDTOById(contentItemId);
            return AddUpdateServiceResponse<DetaieldContentItemDTO>.Success(dto!);
        }

        /// <summary>
        /// الحالة الثلاثية للصورة عند التحديث: إبقاء / استبدال / إزالة.
        /// كل فرع يحفظ بنفسه، ويترك الملف القديم على القرص حتى تنجح قاعدة البيانات.
        /// </summary>
        private async Task<AddUpdateServiceResponse<bool>> ApplyImageChangeAsync(int contentItemId,
            ContentItem contentItem, UploadedFileDTO? file, bool removeImage, int currentUserId)
        {
            // إبقاء: لا استعلام صورة ولا لمس للقرص إطلاقًا
            if (file == null && !removeImage)
            {
                await _unitOfWorkRepository.CompleteAsync();
                return AddUpdateServiceResponse<bool>.Success(true);
            }

            // الصف القديم يُحمَّل متتبَّعًا، ومساره يُلتقط الآن: بعد الحذف لن يبقى صفٌّ نقرأ منه.
            var oldImage = await _unitOfWorkRepository.ImageRepository.GetByContentItemIdAsync(contentItemId);
            var oldRelativePath = oldImage == null ? null : oldImage.RelativePath;

            // إزالة
            if (file == null)
            {
                if (oldImage != null)
                {
                    _unitOfWorkRepository.ImageRepository.Delete(oldImage);
                }

                await _unitOfWorkRepository.CompleteAsync();

                if (oldRelativePath != null)
                {
                    _imageService.DeletePhysicalFile(oldRelativePath);
                }

                return AddUpdateServiceResponse<bool>.Success(true);
            }

            // استبدال
            var prepared = await _imageService.PrepareAsync(file, currentUserId);
            if (!prepared.IsSuccess)
            {
                return AddUpdateServiceResponse<bool>.Failure(prepared.Errors!, prepared.ErrorType!.Value);
            }

            var newImage = prepared.Data!;

            // معاملة صريحة بحفظين، لا حفظ واحد:
            // العلاقة 1:1 تعني فهرسًا فريدًا على Images.ContentItemId، فإدراج الصورة الجديدة
            // قبل أن يصل حذف القديمة إلى القاعدة يصطدم به. الاعتماد على ترتيب EI الضمني
            // للأوامر داخل SaveChanges واحد يجعل ميزةً يراها المستخدم متكئة على سلوك غير موثَّق.
            await _unitOfWorkRepository.BeginTransactionAsync();
            try
            {
                if (oldImage != null)
                {
                    _unitOfWorkRepository.ImageRepository.Delete(oldImage);
                    await _unitOfWorkRepository.CompleteAsync();
                }

                contentItem.Image = newImage;
                await _unitOfWorkRepository.CompleteAsync();

                await _unitOfWorkRepository.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWorkRepository.RollbackTransactionAsync();

                // الملف الجديد فقط يُحذف — القديم ما زال مرجعيًا في قاعدة البيانات بعد التراجع
                _imageService.DeletePhysicalFile(newImage.RelativePath);
                throw;
            }

            // بعد نجاح الالتزام وحده: الآن صار الملف القديم بلا مرجع
            if (oldRelativePath != null)
            {
                _imageService.DeletePhysicalFile(oldRelativePath);
            }

            return AddUpdateServiceResponse<bool>.Success(true);
        }

        // ------------------------------------------------------------------ delete

        public async Task<bool> Delete(int contentItemId)
        {
            var contentItem = await _unitOfWorkRepository.ContentRepository.GetEntityById(contentItemId);
            if (contentItem == null)
                return false;

            // المسار يُلتقط قبل الحذف — Cascade سيزيل الصف ولن يبقى ما نقرأ منه
            var image = await _unitOfWorkRepository.ImageRepository.GetByContentItemIdAsync(contentItemId);
            var relativePath = image == null ? null : image.RelativePath;

            var result = _unitOfWorkRepository.ContentRepository.Delete(contentItem);
            if (!result)
                return false;

            await _unitOfWorkRepository.CompleteAsync();

            // القاعدة أولًا ثم الملف — عكس ترتيب الإنشاء، ولنفس السبب:
            // فشل حذف الملف يترك مساحة ضائعة، أما العكس فيترك صفًّا يشير إلى العدم.
            if (relativePath != null)
            {
                _imageService.DeletePhysicalFile(relativePath);
            }

            return true;
        }

        // ------------------------------------------------------------------ scope resolution

        /// <summary>
        /// أعمق قاعدة عمل في الميزة، وثلاث مهام في استدعاء واحد لكل مستوى:
        ///   (أ) إثبات وجود الكيان المستهدف،
        ///   (ب) إثبات أن الهدف أبٌ للفاعل أو كيانه نفسه — فمسؤول القسم ينشر لقسمه
        ///       أو لكليته أو لجامعته، ولا ينشر لقسم آخر ولا لكلية أخرى،
        ///   (ج) إرجاع سلسلة الأجداد كاملة لتُملأ أعمدة ContentItem الأربعة.
        ///
        /// المهام الثلاث موجودة أصلًا في المشروع: أربع دوال GetXxxAuthorizationInfo تُرجع
        /// السلسلة الكاملة في استعلام واحد، وأربعة مُحوِّلات تُنتج GeneralAuthorizationInfo
        /// بحقول مطابقة تمامًا للأعمدة المطلوبة. المحاذاة مع قيد CHECK دقيقة مستوًى بمستوى.
        /// </summary>
        private async Task<AddUpdateServiceResponse<GeneralAuthorizationInfo>> ResolveScopeTargetAsync(
            UserScope? scope, EnContentScope contentScope, int? targetId)
        {
            if (contentScope == EnContentScope.Public)
            {
                // من ينشر للعالم كله يجب أن يملك العالم كله
                if (scope != null && !scope.IsGlobal)
                {
                    return AddUpdateServiceResponse<GeneralAuthorizationInfo>.Failure(
                        new List<string> { "Only a system administrator may publish public content." },
                        EnErrorTypes.InvalidData);
                }

                return AddUpdateServiceResponse<GeneralAuthorizationInfo>.Success(new GeneralAuthorizationInfo());
            }

            // المُتحقِّق ضمن أن TargetId ليس null لكل نطاق غير عام
            int id = targetId!.Value;

            switch (contentScope)
            {
                case EnContentScope.University:
                    {
                        var info = await _unitOfWorkRepository.UniversityRepository.GetUniversityAuthorizationInfoAsync(id);
                        if (info == null)
                            return AddUpdateServiceResponse<GeneralAuthorizationInfo>.ResourceDoesntExist<University>();

                        var target = info.ToUniversityInfo();
                        if (!scope.IsAncestorOfActor(EnContentScope.University, target))
                            return AddUpdateServiceResponse<GeneralAuthorizationInfo>.ResourceDoesntExist<University>();

                        return AddUpdateServiceResponse<GeneralAuthorizationInfo>.Success(target);
                    }

                case EnContentScope.College:
                    {
                        // بلا لاحقة Async — الوحيدة بين الأربع، والاتساق مسألة وحدة الكليات لا هذه
                        var info = await _unitOfWorkRepository.CollegeRepository.GetCollegeAuthorizationInfo(id);
                        if (info == null)
                            return AddUpdateServiceResponse<GeneralAuthorizationInfo>.ResourceDoesntExist<College>();

                        var target = info.ToCollegeInfo();
                        if (!scope.IsAncestorOfActor(EnContentScope.College, target))
                            return AddUpdateServiceResponse<GeneralAuthorizationInfo>.ResourceDoesntExist<College>();

                        return AddUpdateServiceResponse<GeneralAuthorizationInfo>.Success(target);
                    }

                case EnContentScope.Department:
                    {
                        var info = await _unitOfWorkRepository.DepartmentRepository.GetDepartmentAuthorizationInfoAsync(id);
                        if (info == null)
                            return AddUpdateServiceResponse<GeneralAuthorizationInfo>.ResourceDoesntExist<Department>();

                        var target = info.ToDepartmentInfo();
                        if (!scope.IsAncestorOfActor(EnContentScope.Department, target))
                            return AddUpdateServiceResponse<GeneralAuthorizationInfo>.ResourceDoesntExist<Department>();

                        return AddUpdateServiceResponse<GeneralAuthorizationInfo>.Success(target);
                    }

                case EnContentScope.Batch:
                    {
                        var info = await _unitOfWorkRepository.BatchRepository.GetBatchAuthorizationInfoAsync(id);
                        if (info == null)
                            return AddUpdateServiceResponse<GeneralAuthorizationInfo>.ResourceDoesntExist<Batch>();

                        var target = info.ToBatchInfo();
                        if (!scope.IsAncestorOfActor(EnContentScope.Batch, target))
                            return AddUpdateServiceResponse<GeneralAuthorizationInfo>.ResourceDoesntExist<Batch>();

                        return AddUpdateServiceResponse<GeneralAuthorizationInfo>.Success(target);
                    }

                default:
                    return AddUpdateServiceResponse<GeneralAuthorizationInfo>.Failure(
                        new List<string> { "Unsupported content scope." }, EnErrorTypes.InvalidData);
            }
        }
    }
}
