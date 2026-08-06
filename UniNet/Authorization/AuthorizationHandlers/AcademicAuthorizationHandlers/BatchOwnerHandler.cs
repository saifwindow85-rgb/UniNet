using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Contracts.Common.Extensions;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using UniNet.Authorization.AuthorizationRequirements;
using UniNet.Extensions;

namespace UniNet.Authorization.AuthorizationHandlers.AcademicAuthorizationHandlers
{
    public class BatchOwnerHandler : AuthorizationHandler<OwnershipRequirement, BatchAuthorizationInfo>
    {
        private readonly ICurrentUserService _currentUserService;

        public BatchOwnerHandler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnershipRequirement requirement, BatchAuthorizationInfo resource)
        {
            var scope = _currentUserService.ToUserScope();
            if(scope.IsWithinScope(resource.UniverseId, resource.CollegeId, resource.DepartmentId, resource.BatchId))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
