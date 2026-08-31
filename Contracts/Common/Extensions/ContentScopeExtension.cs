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
