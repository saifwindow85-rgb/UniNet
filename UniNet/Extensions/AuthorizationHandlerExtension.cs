using Microsoft.AspNetCore.Authorization;
using UniNet.Authorization.AuthorizationHandlers.AcademicAuthorizationHandlers;
using UniNet.Authorization.AuthorizationHandlers.EmployeeHandlers;
using UniNet.Authorization.AuthorizationHandlers.StudentHandler;

namespace UniNet.Extensions
{
    public static  class AuthorizationHandlerExtension
    {
        public static IServiceCollection AuthorizationHandlersToDIContainer(this IServiceCollection services)
        {
            // Academic Authorization Handlers
            services.AddScoped<IAuthorizationHandler, UniversityOwnerHandler>();
            services.AddScoped<IAuthorizationHandler, CollegeOwnerHandler>();
            services.AddScoped<IAuthorizationHandler, DepartmentOwnerHandler>();
            // Employee Authorization Handlers
            services.AddScoped<IAuthorizationHandler, EmployeeOwnerHandler>();
            // Student Authorization Handlers
            services.AddScoped<IAuthorizationHandler, StudentOwnerHandler>();
           
            return services;
        }
    }
}
