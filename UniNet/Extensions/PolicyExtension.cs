using UniNet.Authorization.AuthorizationRequirements;
using Microsoft.Extensions.DependencyInjection;

namespace UniNet.Extensions
{
    public static class PolicyExtension
    {
        // كانت AddAuthorization تُستدعى إحدى عشرة مرة، واحدة لكل سياسة. الاستدعاءات تتراكم
        // على نفس AuthorizationOptions فالسلوك كان صحيحًا، لكنها إحدى عشرة تسجيلة زائدة
        // لنفس الخدمة. استدعاء واحد يقبل الكل — وهذا يجعل إضافة السياسة التالية سطرين لا سبعة.
        private static readonly string[] OwnershipPolicies =
        {
            "CollegeOwnerPolicy",
            "EmployeeOwnerPolicy",
            "StudentOwnerPolicy",
            "UniversityOwnerPolicy",
            "DepartmentOwnerPolicy",
            "BatchOwnerPolicy",
            "SectionOwnerPolicy",
            "SubjectOwnerPolicy",
            "SemesterOwnerPolicy",
            "SectionSubjectOwnerPolicy",
            "StudentResultOwnerPolicy",

            // سياسة المشاهدة: تشترك في نفس OwnershipRequirement لكن حارسها ContentViewHandler
            // الذي يسأل CanViewContent بدل IsWithinScope. الفصل في الحارس لا في المتطلَّب،
            // لأن ASP.NET يوجّه الحُرّاس بنوع المورد (ContentViewInfo) لا باسم السياسة.
            "ContentViewPolicy",
        };

        public static IServiceCollection AddPolicyToDIContainer(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                foreach (var policyName in OwnershipPolicies)
                {
                    options.AddPolicy(policyName, policy => policy.Requirements.Add(new OwnershipRequirement()));
                }

                // خارج الحلقة لأنه المتطلَّب الوحيد غير OwnershipRequirement:
                // إدارة المحتوى لا يجوز أن تُرضى بحارس المشاهدة (انظر ContentManagementRequirement).
                options.AddPolicy("ContentManagePolicy",
                    policy => policy.Requirements.Add(new ContentManagementRequirement()));
            });

            return services;
        }
    }
}
