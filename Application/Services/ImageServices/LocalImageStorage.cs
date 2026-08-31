using Contracts.Common.Options;
using Contracts.Requests.ImageRequests;
using Contracts.Responses.ImageResponses;
using Domain.Interfaces.ImageInterfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.ImageServices
{
    public class LocalImageStorage : IImageStorage
    {
        private readonly ImageStorageOptions _options;

        public LocalImageStorage(IOptions<ImageStorageOptions> options)
        {
            _options = options.Value;
        }

        public async Task<StoredFileResult> SaveAsync(UploadedFileDTO file, CancellationToken cancellationToken = default)
        {
            // الامتداد يُشتق من اسم المستخدم لكنه مُتحقَّق منه ضد القائمة البيضاء في المُتحقِّق،
            // ثم يُهجَر الاسم نفسه تمامًا: اسم القرص GUID خالص.
            // Path.Combine لا يحمي من "../../appsettings.json" — بل يتجاهل الجزء الأول لو كان الثاني مطلقًا.
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var storedFileName = $"{Guid.NewGuid():N}{extension}";

            // تقسيم سنة/شهر: مجلد واحد بعشرات آلاف الملفات يُبطئ نظام الملفات بشدة عند السرد.
            var now = DateTime.UtcNow;
            var relativeFolder = Path.Combine("content", now.ToString("yyyy"), now.ToString("MM"));
            var absoluteFolder = Path.Combine(_options.RootPath, relativeFolder);
            Directory.CreateDirectory(absoluteFolder);

            var absolutePath = Path.Combine(absoluteFolder, storedFileName);

            // CreateNew لا Create: تصادم GUID مستحيل عمليًا، والفشل الصريح أفضل من الكتابة فوق ملف قائم.
            // useAsync: العبء هنا إدخال/إخراج لا معالجة، فلا نحجز خيطًا من التجمّع أثناء الكتابة.
            await using var target = new FileStream(absolutePath, FileMode.CreateNew, FileAccess.Write,
                                                    FileShare.None, bufferSize: 81920, useAsync: true);

            // نسخ متدفّق لا ReadAllBytes: 5 ميجابايت × 20 طلبًا متزامنًا = 100 ميجابايت في LOH.
            await file.Content.CopyToAsync(target, cancellationToken);

            return new StoredFileResult
            {
                StoredFileName = storedFileName,
                // فاصل '/' لا فاصل النظام: القيمة تُخزَّن في قاعدة البيانات وقد تُقرأ على نظام آخر.
                RelativePath = Path.Combine(relativeFolder, storedFileName).Replace(Path.DirectorySeparatorChar, '/'),
            };
        }

        public void Delete(string relativePath)
        {
            var absolutePath = GetAbsolutePath(relativePath);

            // حارس أخير ضد Path Traversal: لا نحذف شيئًا خارج جذر التخزين مهما كان محتوى العمود.
            var root = Path.GetFullPath(_options.RootPath);
            if (!absolutePath.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                return;

            if (File.Exists(absolutePath))
                File.Delete(absolutePath);
        }

        public string GetAbsolutePath(string relativePath)
        {
            return Path.GetFullPath(Path.Combine(_options.RootPath, relativePath));
        }
    }
}
