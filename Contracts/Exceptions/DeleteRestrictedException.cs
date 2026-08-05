using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Exceptions
{
    public class DeleteRestrictedException : Exception
    {
        public DeleteRestrictedException(string message) : base(message) { }

        // يحفظ الاستثناء الأصلي (رقم الخطأ SQL، اسم القيد المخالف...) بدل فقدانه
        public DeleteRestrictedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
