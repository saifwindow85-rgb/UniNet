using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Exceptions
{
    /// <summary>
    /// طلبان تنافسا على نفس الصفّ فخسر أحدهما (DbUpdateConcurrencyException).
    ///
    /// أول مسار في التطبيق يجعل هذا واردًا هو استبدال صورة المحتوى: مسؤولان يُحدِّثان
    /// العنصر نفسه، فيُحمِّل كلاهما صفّ الصورة القديم متتبَّعًا ويطلب حذفه، فيجد الثاني
    /// أن الحذف أثّر في صفر صفوف.
    ///
    /// DbUpdateConcurrencyException يرث DbUpdateException لكن استثناءه الداخلي ليس
    /// SqlException، فلا يلتقطه أيٌّ من مُرشِّحَي 547 و 2601/2627 — وكان يخرج كـ 500
    /// مُسجَّلًا كعطل برمجي، بينما هو تسابق حميد جوابه 409.
    /// </summary>
    public class ConcurrentModificationException : Exception
    {
        public ConcurrentModificationException(string message) : base(message) { }

        public ConcurrentModificationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
