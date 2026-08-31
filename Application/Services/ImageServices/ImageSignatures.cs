using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.ImageServices
{
    /// <summary>
    /// فحص التوقيع (Magic Number) — خط الدفاع الحقيقي الوحيد.
    /// الامتداد و ContentType كلاهما يرسلهما العميل ويتحكم بهما بالكامل: مَن يرفع shell.aspx
    /// باسم photo.jpg وترويسة image/jpeg يمرّ من كل فحص وصفي. أول بايتات الملف هي وحدها
    /// ما لا يستطيع تزويره دون أن يُنتج ملفًا صورةً فعلًا.
    /// </summary>
    internal static class ImageSignatures
    {
        private static readonly Dictionary<string, byte[]> Prefixes = new(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = new byte[] { 0xFF, 0xD8, 0xFF },
            [".jpeg"] = new byte[] { 0xFF, 0xD8, 0xFF },
            [".png"] = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A },
        };

        // WEBP لا يوضع في الجدول أعلاه لأن توقيعه ليس بادئة متصلة:
        // "RIFF" في البايتات 0..3 ثم حجم الملف في 4..7 ثم "WEBP" في 8..11.
        private static readonly byte[] Riff = { 0x52, 0x49, 0x46, 0x46 };
        private static readonly byte[] Webp = { 0x57, 0x45, 0x42, 0x50 };

        private const int HeaderLength = 12;

        public static async Task<bool> MatchesAsync(Stream content, string extension,
            CancellationToken cancellationToken = default)
        {
            // الفشل المغلق: بلا Seek لا يمكن قراءة الترويسة ثم إعادة المؤشر للنسخ، فنرفض.
            // عمليًا لا يحدث: ASP.NET يُخزّن ملف النموذج مؤقتًا (ذاكرة ثم ملف) قبل تسليمه.
            if (!content.CanSeek)
                return false;

            var header = new byte[HeaderLength];
            content.Seek(0, SeekOrigin.Begin);
            int read = await content.ReadAtLeastAsync(header, HeaderLength, throwOnEndOfStream: false, cancellationToken);
            content.Seek(0, SeekOrigin.Begin);

            if (".webp".Equals(extension, StringComparison.OrdinalIgnoreCase))
            {
                return read >= HeaderLength
                    && header.AsSpan(0, 4).SequenceEqual(Riff)
                    && header.AsSpan(8, 4).SequenceEqual(Webp);
            }

            if (!Prefixes.TryGetValue(extension, out var signature))
                return false;

            return read >= signature.Length && header.AsSpan(0, signature.Length).SequenceEqual(signature);
        }
    }
}
