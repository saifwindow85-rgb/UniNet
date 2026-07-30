using Contracts.Common.Extensions;
using Contracts.Responses.EmployeeResponse;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using UniNet.Authorization.AuthorizationRequirements;
using UniNet.Extensions;

namespace UniNet.Authorization.AuthorizationHandlers.EmployeeHandlers
{
    public class EmployeeHandler : AuthorizationHandler<OwnershipRequirement,EmployeeAuthorizationInfo>
    {
        private readonly ICurrentUserService _currentUserService;

        public EmployeeHandler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnershipRequirement requirement, EmployeeAuthorizationInfo resource)
        {
            var scope = _currentUserService.ToEmployeeScope();
            if (scope.IsWithinScope(resource.UniversityId, resource.CollegeId, resource.DepartmentId))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
