using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.ContentRequests
{
    /// <summary>
    /// بلا Scope وبلا TargetId: النطاق ثابت بعد الإنشاء.
    /// تغييره يعني تغيير جمهور المحتوى، وهو تصعيد صلاحيات مقنَّع يحتاج تفويضًا على الجمهورين
    /// القديم والجديد معًا. والسابقة في البيت نفسه: UpdateSubjectDTO لا يحمل DepartmentId.
    /// من أراد نطاقًا آخر يُنشئ محتوى جديدًا.
    /// </summary>
    public class UpdateContentDTO
    {
        public string Title { get; set; } = null!;

        public string Body { get; set; } = null!;

        /// <summary>
        /// الحالة الثالثة. الملف وحده لا يكفي: file == null غامض بين إبقاء الصورة وحذفها.
        ///   لا ملف و RemoveImage = false ⇒ إبقاء
        ///   ملف موجود                    ⇒ استبدال
        ///   لا ملف و RemoveImage = true  ⇒ إزالة
        /// </summary>
        public bool RemoveImage { get; set; }
    }
}
