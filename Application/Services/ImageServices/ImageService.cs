using Contracts.Enums;
using Contracts.Requests.ImageRequests;
using Contracts.Responses;
using Contracts.Responses.ImageResponses;
using Domain.Entities.Images;
using Domain.Interfaces.ImageInterfaces;
using Domain.Interfaces.UnitOfWork;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.ImageServices
{
    public class ImageService : IImageService
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly IImageStorage _imageStorage;
        private readonly IValidator<UploadedFileDTO> _fileValidator;

        public ImageService(IUnitOfWorkRepository unitOfWorkRepository, IImageStorage imageStorage,
            IValidator<UploadedFileDTO> fileValidator)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
            _imageStorage = imageStorage;
            _fileValidator = fileValidator;
        }

        public async Task<AddUpdateServiceResponse<Image>> PrepareAsync(UploadedFileDTO file, int currentUserId)
        {
            var validationResult = await _fileValidator.ValidateAsync(file);
            if (!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<Image>.Failure(
                    validationResult.Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(),
                    EnErrorTypes.InvalidData);
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!await ImageSignatures.MatchesAsync(file.Content, extension))
            {
                return AddUpdateServiceResponse<Image>.Failure(
                    new List<string> { "The file content does not match its extension. Only real image files are accepted." },
                    EnErrorTypes.InvalidData);
            }

            // الملف أولًا ثم قاعدة البيانات — فوضع الفشل الوحيد الممكن هو "ملف يتيم" (مساحة ضائعة
            // قابلة للتنظيف) لا "صف يشير إلى العدم" (صورة مكسورة أمام كل مستخدم للأبد).
            var stored = await _imageStorage.SaveAsync(file);

            return AddUpdateServiceResponse<Image>.Success(new Image
            {
                // GetFileName يجرّد أي مسار أرسله المتصفح — الاسم للعرض فقط ولا يُبنى منه مسار.
                OriginalFileName = Path.GetFileName(file.FileName),
                StoredFileName = stored.StoredFileName,
                RelativePath = stored.RelativePath,
                ContentType = file.ContentType,
                FileSize = file.Length,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = currentUserId,
            });
        }

        public void DeletePhysicalFile(Image image)
        {
            _imageStorage.Delete(image.RelativePath);
        }

        public async Task<ImageFileDTO?> GetFileInfoByContentItemIdAsync(int contentItemId)
        {
            return await _unitOfWorkRepository.ImageRepository.GetFileInfoByContentItemIdAsync(contentItemId);
        }

        public string GetAbsolutePath(string relativePath)
        {
            return _imageStorage.GetAbsolutePath(relativePath);
        }
    }
}
