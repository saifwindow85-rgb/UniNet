using Microsoft.AspNetCore.Authorization;

namespace UniNet.Authorization.AuthorizationRequirements
{
    /// <summary>
    /// متطلَّب مستقل عن OwnershipRequirement، ولهذا سببٌ تقني لا ذوقي.
    ///
    /// ASP.NET Core يستدعي كل IAuthorizationHandler يطابق نوع المورد، ونجاح أيٍّ منها
    /// على نسخة المتطلَّب يُرضي السياسة كلها. لو تشارك ContentViewHandler و ContentManageHandler
    /// النوعين نفسيهما (OwnershipRequirement + مورد واحد)، لكان مجرّد حقّ المشاهدة
    /// كافيًا لاجتياز سياسة الإدارة. الفصل بنوع المتطلَّب ونوع المورد معًا يجعل
    /// هذا الالتباس مستحيلًا نحويًا لا اعتمادًا على انضباط من يكتب الحارس التالي.
    /// </summary>
    public class ContentManagementRequirement : IAuthorizationRequirement
    {
    }
}
