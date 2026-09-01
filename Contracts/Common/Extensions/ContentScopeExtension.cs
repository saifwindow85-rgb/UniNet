using Contracts.Common.AuthorizationInfos;
using Contracts.Common.AuthorizationInfos.ContentAuthorizationInfo;
using Contracts.Enums;
using Contracts.Requests.RequestParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Common.Extensions
{
    public static class ContentScopeExtension
    {
        /// <summary>
        /// هل الجهة المستهدَفة أبٌ للفاعل — أو هي كيانه نفسه عند ذلك المستوى؟
        ///
        /// هذا مُسنَد الكتابة، ومعكوس IsWithinScope عمدًا:
        ///   IsWithinScope     : "هل المورد داخل نطاقي؟"  ← يصلح لإدارة مورد قائم
        ///   IsAncestorOfActor : "هل الهدف يحويني؟"       ← يصلح لاختيار جمهور منشور
        ///
        /// الفرق ليس تجميليًّا. النطاق يعني «مَن يُسمح له بالرؤية» لا «سقف ما يملكه الكاتب»:
        /// فمسؤول الدفعة قد يكتب خبرًا يخصّ قسمه كلّه أو كليته أو جامعته. IsWithinScope
        /// كانت تقارن عند أعمق مطالبة للفاعل، فترفض هدفًا أعلى منه لأن عموده يكون null
        /// في سلسلة الهدف — أي تحبس كل مسؤول في مستواه وحده.
        ///
        /// هنا تقع المقارنة عند المستوى المطلوب وحده، فيُقبل «كليتي» ويُرفض «كلية أخرى».
        /// </summary>
        public static bool IsAncestorOfActor(this UserScope? actor, EnContentScope scope, GeneralAuthorizationInfo target)
        {
            // فشل مغلق — بخلاف IsWithinScope التي تُرجع true عند غياب النطاق.
            if (actor == null)
                return false;

            if (actor.IsGlobal)
                return true;

            // المقارنة عند المستوى المطلوب فقط: الأعمدة الأعمق من الهدف تكون null في سلسلته،
            // فمقارنتها بمطالبة الفاعل كانت هي بالضبط سبب الحبس في المستوى الواحد.
            return scope switch
            {
                EnContentScope.University => actor.UniversityId is not null && target.UniversityId == actor.UniversityId,
                EnContentScope.College => actor.CollegeId is not null && target.CollegeId == actor.CollegeId,
                EnContentScope.Department => actor.DepartmentId is not null && target.DepartmentId == actor.DepartmentId,
                EnContentScope.Batch => actor.BatchId is not null && target.BatchId == actor.BatchId,

                // Public أوسع من أي نطاق، فلا أحد دون المسؤول العام أبٌ له.
                _ => false,
            };
        }

        /// <summary>
        /// هل يقع المُشاهِد داخل الجمهور المستهدف بهذا المحتوى؟
        ///
        /// هذا سؤال معاكس لسؤال IsWithinScope، ولا يجوز الخلط بينهما:
        ///   IsWithinScope : "هل نطاق هذا المسؤول يحتوي المورد؟"  ← صلاحية الإدارة (تعديل/حذف)
        ///   CanViewContent: "هل المُشاهِد داخل جمهور المحتوى؟"   ← صلاحية المشاهدة
        ///
        /// لو استُعمل IsWithinScope للمشاهدة لانكسر النظام في الاتجاهين:
        ///   • منشور Public معلوماتُه كلها null، وطالبٌ له BatchId ⇒ null == 7 ⇒ false،
        ///     أي أن الطالب لا يرى أي منشور عام إطلاقًا.
        ///   • ومسؤول قسم كان سيُعدّ "داخل النطاق" لمحتوى كليةٍ كاملة.
        /// </summary>
        /// <summary>
        /// هل يملك الفاعل حقّ إدارة (تعديل/حذف) هذا المحتوى؟
        ///
        /// ثالث مُسنَد في المشروع، ولا يُدمج بأيٍّ من الاثنين:
        ///   IsWithinScope  : عام لكل الموارد — ويسقط مفتوحًا هنا (انظر أدناه)
        ///   CanViewContent : جمهور المحتوى — يُرجع true لكل منشور عام
        ///   CanManageContent: هذا
        ///
        /// لماذا لا يُعاد استعمال IsWithinScope حرفيًا: سطره الأخير
        /// «return info.UniversityId == scope.UniversityId;» سقوطٌ نهائي بلا حارس.
        /// فاعلٌ بلا أي مطالبة نطاق (UniversityId = null) يقارن null بـ null على محتوى عام
        /// فيحصل على true — أي أن كل مستخدم بلا جامعة يُدير كل المحتوى العام.
        /// كذلك «scope == null ⇒ true» في أوّله يفتح الباب على مصراعيه عند غياب النطاق.
        /// هنا السقوط مغلق: لا مطالبة ⇒ لا صلاحية.
        /// </summary>
        public static bool CanManageContent(this UserScope? actor, ContentManageInfo content, int currentUserId)
        {
            // فشل مغلق — عكس IsWithinScope التي تُرجع true عند scope == null
            if (actor == null)
                return false;

            if (actor.IsGlobal)
                return true;

            // الكاتب يملك ما كتب. بدون هذا السطر يعجز مسؤول القسم عن تعديل منشوره
            // إن نُقل إلى قسم آخر — و CreatedByUserId لا يقرأه أي مُسنَد آخر في المشروع.
            if (content.CreatedByUserId == currentUserId)
                return true;

            // سلسلة الأجداد منزَّلة على ContentItem، فالاحتواء مقارنة مباشرة بلا ضمّ:
            // مسؤول الجامعة يُدير محتوى كلياتها وأقسامها ودفعاتها لأن UniversityId مملوء فيها كلها.
            // والمحتوى العام تكون أعمدته الأربعة null فلا يطابق أي مطالبة — أي أن إدارته
            // محصورة بالمسؤول العام وبالكاتب، وهذا مقصود.
            if (actor.BatchId.HasValue)
                return content.BatchId == actor.BatchId;

            if (actor.DepartmentId.HasValue)
                return content.DepartmentId == actor.DepartmentId;

            if (actor.CollegeId.HasValue)
                return content.CollegeId == actor.CollegeId;

            if (actor.UniversityId.HasValue)
                return content.UniversityId == actor.UniversityId;

            return false;
        }

        /// <summary>
        /// مرآة CK_ContentItems_ScopeTargets في الكود.
        /// القيد يحرس قاعدة البيانات، لكنه يحرسها بـ SQL error 547 — أي بعد فوات الأوان:
        /// الملف كُتب على القرص، والمستخدم يرى رسالة قيدٍ عامة. هذا المُسنَد يسمح للخدمة
        /// أن ترفض مبكرًا برسالة مفهومة، ويبقى القيد شبكة الأمان الأخيرة لا الحارس الأول.
        /// </summary>
        public static bool IsScopeConsistent(EnContentScope scope, int? universityId, int? collegeId,
            int? departmentId, int? batchId)
        {
            return scope switch
            {
                EnContentScope.Public => universityId == null && collegeId == null && departmentId == null && batchId == null,
                EnContentScope.University => universityId != null && collegeId == null && departmentId == null && batchId == null,
                EnContentScope.College => universityId != null && collegeId != null && departmentId == null && batchId == null,
                EnContentScope.Department => universityId != null && collegeId != null && departmentId != null && batchId == null,
                EnContentScope.Batch => universityId != null && collegeId != null && departmentId != null && batchId != null,
                _ => false,
            };
        }

        public static bool CanViewContent(this UserScope? viewer, ContentViewInfo content)
        {
            // المحتوى العام يراه كل مستخدم مُصادَق عليه — قبل أي فحص آخر.
            if (content.Scope == EnContentScope.Public)
                return true;

            // مسؤول النظام يرى كل شيء.
            if (viewer == null || viewer.IsGlobal)
                return true;

            // المستوى المستهدف وحده هو ما يُقارَن؛ الأعمدة الأعلى مخزَّنة للتفويض الإداري لا للمشاهدة.
            return content.Scope switch
            {
                EnContentScope.University => content.UniversityId == viewer.UniversityId,
                EnContentScope.College => content.CollegeId == viewer.CollegeId,
                EnContentScope.Department => content.DepartmentId == viewer.DepartmentId,
                EnContentScope.Batch => content.BatchId == viewer.BatchId,
                _ => false,
            };
        }
    }
}
