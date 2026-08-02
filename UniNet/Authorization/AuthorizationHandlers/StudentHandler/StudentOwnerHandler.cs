using Contracts.Common.AuthorizationInfos.StudentAuthorizationInfo;
using Contracts.Common.Extensions;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniNet.Authorization.AuthorizationRequirements;
using UniNet.Extensions;

namespace UniNet.Authorization.AuthorizationHandlers.StudentHandler
{
    public class StudentOwnerHandler : AuthorizationHandler<OwnershipRequirement, StudentAuthorizationInfo>
    {
        private readonly ICurrentUserService _currentUserService;
        public StudentOwnerHandler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnershipRequirement requirement, StudentAuthorizationInfo resource)
        {
            var scope = _currentUserService.ToUserScope();
            var isStudent = context.User.IsInRole("Student");
            if (isStudent && _currentUserService.StudentId == resource.StudentId)
            {
                context.Succeed(requirement);
            }

            else if (scope.IsWithinScope(resource.UniversityId, resource.CollegeId, resource.DepartmentId, resource.BatchId))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
