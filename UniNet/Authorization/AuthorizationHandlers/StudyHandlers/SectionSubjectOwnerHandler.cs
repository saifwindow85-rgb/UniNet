using Contracts.Common.AuthorizationInfos.StudyAuthorizationInfos;
using Contracts.Common.Extensions;
using Contracts.Common.Mappers;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using UniNet.Authorization.AuthorizationRequirements;
using UniNet.Extensions;

namespace UniNet.Authorization.AuthorizationHandlers.StudyHandlers
{
    public class SectionSubjectOwnerHandler : AuthorizationHandler<OwnershipRequirement, SectionSubjectAuthorizationInfo>
    {
        private readonly ICurrentUserService _currentUserService;

        public SectionSubjectOwnerHandler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnershipRequirement requirement, SectionSubjectAuthorizationInfo resource)
        {
            var scope = _currentUserService.ToUserScope();
            if (scope.IsWithinScope(resource.ToSectionSubjectInfo()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
