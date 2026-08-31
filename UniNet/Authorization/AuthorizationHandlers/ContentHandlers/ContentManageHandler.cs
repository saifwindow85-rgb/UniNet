using Contracts.Common.AuthorizationInfos.ContentAuthorizationInfo;
using Contracts.Common.Extensions;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using UniNet.Authorization.AuthorizationRequirements;
using UniNet.Extensions;

namespace UniNet.Authorization.AuthorizationHandlers.ContentHandlers
{
    /// <summary>
    /// حارس الإدارة — التوأم المقابل لـ ContentViewHandler.
    /// يختلف عنه في ثلاثة أشياء دفعةً واحدة: نوع المتطلَّب، ونوع المورد، والمُسنَد.
    /// وهو الحارس الوحيد في المشروع الذي يحتاج معرّف المستخدم إلى جانب نطاقه،
    /// لأن ملكية الكاتب لمحتواه جزء من القرار.
    /// </summary>
    public class ContentManageHandler : AuthorizationHandler<ContentManagementRequirement, ContentManageInfo>
    {
        private readonly ICurrentUserService _currentUserService;

        public ContentManageHandler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ContentManagementRequirement requirement, ContentManageInfo resource)
        {
            if (_currentUserService.ToUserScope().CanManageContent(resource, _currentUserService.UserId))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
