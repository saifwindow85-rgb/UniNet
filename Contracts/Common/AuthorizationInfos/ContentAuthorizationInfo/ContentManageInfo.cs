using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Common.AuthorizationInfos.ContentAuthorizationInfo
{
    /// <summary>
    /// معلومات تفويض الإدارة (تعديل/حذف) — نوع مستقل تمامًا عن ContentViewInfo، وليس ترفًا:
    /// ASP.NET Core يوجّه حُرّاس التفويض بنوع المورد لا باسم السياسة. لو تشارك حارس المشاهدة
    /// وحارس الإدارة النوع نفسه لعمل الاثنان على كل استدعاء، ونجاح أحدهما يُرضي المتطلَّب كاملًا —
    /// فتصير سياسة الإدارة قابلة للتحقق بمجرد امتلاك حق المشاهدة.
    ///
    /// ويحمل CreatedByUserId الذي لا يقرأه أي مُسنَد في المشروع حاليًا: بدونه لا يملك أحد
    /// محتواه، فيصير كاتب المنشور عاجزًا عن تعديل ما كتبه إن خرج عن نطاقه الإداري.
    /// </summary>
    public class ContentManageInfo
    {
        public EnContentScope Scope { get; set; }

        public int? UniversityId { get; set; }

        public int? CollegeId { get; set; }

        public int? DepartmentId { get; set; }

        public int? BatchId { get; set; }

        // غير قابل للعدم: BaseEntity.CreatedByUserId نفسه غير قابل للعدم،
        // وإسقاطه كـ int? كان سيسمح لـ null بأن يُقرأ بمعنى بلا كاتب.
        public int CreatedByUserId { get; set; }
    }
}
