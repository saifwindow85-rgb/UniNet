using Contracts.Common.AuthorizationInfos.EmployeeAuthorizationInfo;
using Contracts.Common.Extensions;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using UniNet.Authorization.AuthorizationRequirements;
using UniNet.Extensions;

namespace UniNet.Authorization.AuthorizationHandlers.EmployeeHandlers
{
    public class EmployeeOwnerHandler : AuthorizationHandler<OwnershipRequirement,EmployeeAuthorizationInfo>
    {
        private readonly ICurrentUserService _currentUserService;

        public EmployeeOwnerHandler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnershipRequirement requirement, EmployeeAuthorizationInfo resource)
        {
            var scope = _currentUserService.ToUserScope();
            if (scope.IsWithinScope(resource.UniversityId, resource.CollegeId, resource.DepartmentId, null))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
