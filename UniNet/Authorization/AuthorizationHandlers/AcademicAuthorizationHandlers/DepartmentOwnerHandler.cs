using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Contracts.Common.Extensions;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using UniNet.Authorization.AuthorizationRequirements;
using UniNet.Extensions;

namespace UniNet.Authorization.AuthorizationHandlers.AcademicAuthorizationHandlers
{
    public class DepartmentOwnerHandler : AuthorizationHandler<OwnershipRequirement, DepartmentAuthorizationInfo>
    {
        private readonly ICurrentUserService _currentUserService;

        public DepartmentOwnerHandler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnershipRequirement requirement, DepartmentAuthorizationInfo department    )
        {
            var scope = _currentUserService.ToUserScope();
            if(scope.IsWithinScope(department.UniversityId,department.CollegeId,department.DepartmentId))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
