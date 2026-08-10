using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Contracts.Common.Extensions;
using Contracts.Common.Mappers;
using Domain.Entities.Academic_Structure;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using UniNet.Authorization.AuthorizationRequirements;
using UniNet.Extensions;

namespace UniNet.Authorization.AuthorizationHandlers.AcademicAuthorizationHandlers
{
    public class CollegeOwnerHandler : AuthorizationHandler<OwnershipRequirement,CollegeAuthorizationInfo>
    {
        private readonly ICurrentUserService _currentUserService;

        public CollegeOwnerHandler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnershipRequirement requirement, CollegeAuthorizationInfo college)
        {

            var scope = _currentUserService.ToUserScope();
            if(scope.IsWithinScope(college.ToCollegeInfo()))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
