using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Contracts.Common.Extensions;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using UniNet.Authorization.AuthorizationRequirements;
using UniNet.Extensions;

namespace UniNet.Authorization.AuthorizationHandlers.AcademicAuthorizationHandlers
{
    public class SectionOwnerHandler : AuthorizationHandler<OwnershipRequirement, SectionAuthorizationInfo>
    {
        private readonly ICurrentUserService _currentUserService;
        public  SectionOwnerHandler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnershipRequirement requirement, SectionAuthorizationInfo resource)
        {
            var scope = _currentUserService.ToUserScope();
            if (scope.IsWithinScope(resource.UniversityId, resource.CollegeId, resource.DepartmentId, resource.BatchId))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask; 
        }
    }
}
