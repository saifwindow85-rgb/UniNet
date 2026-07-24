using Domain.Entities.Academic_Structure;
using Microsoft.AspNetCore.Authorization;
using UniNet.Authorization.AuthorizationRequirements;

namespace UniNet.Authorization.AuthorizationHandlers
{
    public class CollegeOwnerHandler : AuthorizationHandler<OwnershipRequirement,College>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnershipRequirement requirement, College college)
        {
            var universityId = context.User.FindFirst("UniversityId")?.Value;
            var role = context.User.FindFirst("Super Admin")?.Value;
            if(college.UniversityId.ToString() == universityId|| role != null)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
