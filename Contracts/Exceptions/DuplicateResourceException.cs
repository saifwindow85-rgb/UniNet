using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Exceptions
{
    /// <summary>
    /// كتابة خالفت فهرسًا فريدًا (أخطاء SQL Server 2601 و 2627).
    /// أضيفت لأن مسار استبدال صورة المحتوى يصطدم بـ IX_Images_ContentItemId الفريد،
    /// وفلتر CompleteAsync كان يفحص 547 وحده — فيهرب التصادم إلى الوسيط كـ 500 عارٍ
    /// بلا أي دلالة على السبب.
    /// </summary>
    public class DuplicateResourceException : Exception
    {
        public DuplicateResourceException(string message) : base(message) { }

        public DuplicateResourceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
