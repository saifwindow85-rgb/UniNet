using Contracts.Common.Options;
using Contracts.Requests.ImageRequests;
using FluentValidation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ImageValidators
{
    /// <summary>
    /// بيانات وصفية فقط: حجم، امتداد، نوع محتوى.
    /// فحص التوقيع مقصود أنه ليس هنا — فهو يقرأ الـ Stream ويحرّك مؤشره، وهذا أثر جانبي
    /// لا يليق بمُتحقِّق يُفترض أن يكون نقيًا. مكانه ImageService قبل استدعاء التخزين.
    /// </summary>
    public class UploadedFileValidator : AbstractValidator<UploadedFileDTO>
    {
        public UploadedFileValidator(IOptions<ImageStorageOptions> options)
        {
            var storage = options.Value;

            RuleFor(f => f.Length)
                .GreaterThan(0).WithMessage("The uploaded file is empty.")
                .LessThanOrEqualTo(storage.MaxFileSizeInBytes)
                .WithMessage($"Maximum allowed size is {storage.MaxFileSizeInBytes / 1024 / 1024} MB.");

            RuleFor(f => f.FileName)
                .NotEmpty().WithMessage("File name is required.")
                .Must(name => storage.AllowedExtensions.Contains(Path.GetExtension(name).ToLowerInvariant()))
                .WithMessage($"Allowed extensions: {string.Join(", ", storage.AllowedExtensions)}");

            RuleFor(f => f.ContentType)
                .NotEmpty().WithMessage("Content type is required.")
                .Must(type => storage.AllowedContentTypes.Contains(type.ToLowerInvariant()))
                .WithMessage($"Allowed content types: {string.Join(", ", storage.AllowedContentTypes)}");
        }
    }
}
