using Contracts.Common.AuthorizationInfos.ContentAuthorizationInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses.ImageResponses
{
    // كل ما تحتاجه نقطة العرض في استعلام واحد: بيانات البثّ + جمهور المحتوى المالك.
    // ضمّ معلومات التفويض هنا يمنع استعلامًا ثانيًا لجلب المالك بعد جلب الصورة.
    public class ImageFileDTO
    {
        public int ImageId { get; set; }
        public string RelativePath { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public string OriginalFileName { get; set; } = null!;
        public ContentViewInfo ViewInfo { get; set; } = null!;
    }
}
