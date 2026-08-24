using Contracts.Common.AuthorizationInfos.StudyAuthorizationInfos;
using Contracts.Common.Extensions;
using Contracts.Common.Mappers;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using UniNet.Authorization.AuthorizationRequirements;
using UniNet.Extensions;

namespace UniNet.Authorization.AuthorizationHandlers.StudyHandlers
{
    public class SemesterOwnerHandler : AuthorizationHandler<OwnershipRequirement, SemesterAuthorizationInfo>
    {
        private readonly ICurrentUserService _currentUserService;

        public SemesterOwnerHandler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnershipRequirement requirement, SemesterAuthorizationInfo resource)
        {
            var scope = _currentUserService.ToUserScope();
            if (scope.IsWithinScope(resource.ToSemesterInfo()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
