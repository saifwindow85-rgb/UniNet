using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using UniNet.Authorization.AuthorizationRequirements;

namespace UniNet.Authorization.AuthorizationHandlers.AcademicAuthorizationHandlers
{
    public class UniversityOwnerHandler : AuthorizationHandler<OwnershipRequirement, UniversityAuthorizationInfo>
    {
        private ICurrentUserService _currentUserService;
        public UniversityOwnerHandler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }
       
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnershipRequirement requirement, UniversityAuthorizationInfo resource)
        {
          var IsSuperAdmin = context.User.IsInRole("Super Admin");
            if(_currentUserService.UniversityId == resource.UniversityId || IsSuperAdmin)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
