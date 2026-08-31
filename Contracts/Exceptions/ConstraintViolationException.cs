using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Exceptions
{
    /// <summary>
    /// كتابة (INSERT/UPDATE) خالفت قيد CHECK أو مفتاحًا أجنبيًا.
    /// تُميَّز عن DeleteRestrictedException رغم أن SQL Server يُطلق الرقم 547 لكلتيهما:
    /// 547 نصّه "The %s statement conflicted with the %s constraint" — و%s الأولى قد تكون
    /// DELETE أو INSERT أو UPDATE. ترجمة كل 547 إلى "لا يمكن الحذف" كانت تُظهر للمستخدم
    /// رسالة حذفٍ على عملية إنشاء محتوى بنطاق غير متسق.
    /// </summary>
    public class ConstraintViolationException : Exception
    {
        public ConstraintViolationException(string message) : base(message) { }

        public ConstraintViolationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
