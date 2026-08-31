using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses.ImageResponses
{
    // ما تُرجعه طبقة التخزين بعد كتابة الملف على القرص: الاسم المولَّد والمسار النسبي.
    // لا يحمل ContentItemId لأن التخزين لا يعرف شيئًا عن المحتوى — هذا مقصود.
    public class StoredFileResult
    {
        public string StoredFileName { get; set; } = null!;
        public string RelativePath { get; set; } = null!;
    }
}
