using Contracts.Common.AuthorizationInfos.ContentAuthorizationInfo;
using Contracts.Common.Extensions;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using UniNet.Authorization.AuthorizationRequirements;
using UniNet.Extensions;

namespace UniNet.Authorization.AuthorizationHandlers.ContentHandlers
{
    /// <summary>
    /// حارس المشاهدة — لا حارس ملكية.
    /// بقية الحُرّاس تسأل IsWithinScope ("هل نطاق المسؤول يحتوي المورد؟") وهو سؤال الإدارة.
    /// هذا يسأل CanViewContent ("هل المُشاهِد داخل جمهور المحتوى؟") وهو سؤال المشاهدة.
    /// حين تصل نقاط تعديل/حذف المحتوى ستحتاج حارسًا ثانيًا يستعمل IsWithinScope — ولا يُدمَجان.
    /// </summary>
    public class ContentViewHandler : AuthorizationHandler<OwnershipRequirement, ContentViewInfo>
    {
        private readonly ICurrentUserService _currentUserService;

        public ContentViewHandler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            OwnershipRequirement requirement, ContentViewInfo resource)
        {
            if (_currentUserService.ToUserScope().CanViewContent(resource))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
