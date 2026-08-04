using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Domain.Entities.Academic_Structure;
using Microsoft.AspNetCore.Authorization;
using UniNet.Authorization.AuthorizationRequirements;

namespace UniNet.Authorization.AuthorizationHandlers.AcademicAuthorizationHandlers
{
    public class CollegeOwnerHandler : AuthorizationHandler<OwnershipRequirement,CollegeAuthorizationInfo>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnershipRequirement requirement, CollegeAuthorizationInfo college)
        {
            var universityId = context.User.FindFirst("UniversityId")?.Value;
            var IsSuperAdmin = context.User.IsInRole("Super Admin");
            if(college.UniversityId.ToString() == universityId|| IsSuperAdmin)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
