using UniNet.Authorization.AuthorizationRequirements;
using Microsoft.Extensions.DependencyInjection;
namespace UniNet.Extensions
{
    public static class PolicyExtension
    {
        public static IServiceCollection AddPolicyToDIContainer(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CollegeOwnerPolicy", policy =>
                    policy.Requirements.Add(new OwnershipRequirement()));
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("EmployeeOwnerPolicy", policy =>
                    policy.Requirements.Add(new OwnershipRequirement()));
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("StudentOwnerPolicy", policy =>
                    policy.Requirements.Add(new OwnershipRequirement()));
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("UniversityOwnerPolicy", policy =>
                    policy.Requirements.Add(new OwnershipRequirement()));
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("DepartmentOwnerPolicy", policy =>
                    policy.Requirements.Add(new OwnershipRequirement()));
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("BatchOwnerPolicy", policy =>
                    policy.Requirements.Add(new OwnershipRequirement()));
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("SectionOwnerPolicy", policy =>
                    policy.Requirements.Add(new OwnershipRequirement()));
            });
            return services;
        }
    }
}
