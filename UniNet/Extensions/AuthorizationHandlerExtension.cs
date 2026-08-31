using Microsoft.AspNetCore.Authorization;
using UniNet.Authorization.AuthorizationHandlers.AcademicAuthorizationHandlers;
using UniNet.Authorization.AuthorizationHandlers.ContentHandlers;
using UniNet.Authorization.AuthorizationHandlers.EmployeeHandlers;
using UniNet.Authorization.AuthorizationHandlers.StudentHandler;
using UniNet.Authorization.AuthorizationHandlers.StudyHandlers;

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
            services.AddScoped<IAuthorizationHandler, BatchOwnerHandler>();
            services.AddScoped<IAuthorizationHandler, SectionOwnerHandler>();
            // Employee Authorization Handlers
            services.AddScoped<IAuthorizationHandler, EmployeeOwnerHandler>();
            // Student Authorization Handlers
            services.AddScoped<IAuthorizationHandler, StudentOwnerHandler>();
            // Study Authorization Handlers
            services.AddScoped<IAuthorizationHandler, SubjectOwnerHandler>();
            services.AddScoped<IAuthorizationHandler, SemesterOwnerHandler>();
            services.AddScoped<IAuthorizationHandler, SectionSubjectOwnerHandler>();
            services.AddScoped<IAuthorizationHandler, StudentResultOwnerHandler>();
            // Content Authorization Handlers
            services.AddScoped<IAuthorizationHandler, ContentViewHandler>();

            return services;
        }
    }
}
