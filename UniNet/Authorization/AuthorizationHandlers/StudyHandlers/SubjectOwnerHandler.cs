using Contracts.Common.AuthorizationInfos.StudyAuthorizationInfos;
using Contracts.Common.Extensions;
using Contracts.Common.Mappers;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using UniNet.Authorization.AuthorizationRequirements;
using UniNet.Extensions;

namespace UniNet.Authorization.AuthorizationHandlers.StudyHandlers
{
    public class SubjectOwnerHandler : AuthorizationHandler<OwnershipRequirement, SubjectAuthorizationInfo>
    {
        private readonly ICurrentUserService _currentUserService;

        public SubjectOwnerHandler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnershipRequirement requirement, SubjectAuthorizationInfo resource)
        {
            var scope = _currentUserService.ToUserScope();
            if (scope.IsWithinScope(resource.ToSubjectInfo()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
