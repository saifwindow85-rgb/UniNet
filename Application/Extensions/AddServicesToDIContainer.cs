using Application.Services.IdentityServices;
using Domain.Interfaces.LoginInterfaces;
using Domain.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Validators.LoginRequestValidator;
using Contracts.Requests.LoginRequests;
using Contracts.Requests.UserRequests;
using Application.Validators;
using Domain.Interfaces.LoginInterfaces.TokenInterfaces;
using Application.Services.Login_Service;
using Application.Validators.User_Validators;
using Contracts.Requests.IdentityRequests.RoleRequests;
using Application.Validators.IdentityValidators.RoleValidators;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Domain.Interfaces.IdentityInterfaces.RoleInterfaces;
using Domain.Interfaces.IdentityInterfaces.UserRoleInterfaces;
using Domain.Interfaces.AcademicStructureInterfaces.UniversityInterfaces;
using Application.Services.AcademicServices;
using Contracts.Requests.AcademicRequests.UniversityRequests;
using Application.Validators.AcademicValidators.UniversityValidators;
using Contracts.Requests.AcademicRequests.CollegeRequests;
using Application.Validators.AcademicValidators.CollegeValidators;
using Domain.Interfaces.AcademicStructureInterfaces.CollegeInterfaces;
using Contracts.Requests.AcademicRequests.DepartmentRequests;
using Application.Validators.AcademicValidators.DepartmentValidators;
using Domain.Interfaces.AcademicStructureInterfaces.DepartmentInterfaces;
using Domain.Interfaces.AcademicStructureInterfaces.BatchInterfaces;
using Contracts.Requests.AcademicRequests.BatchRequests;
using Application.Validators.AcademicValidators.BatchValidators;
using Contracts.Requests.AcademicRequests.SectionRequests;
using Application.Validators.AcademicValidators.SectionValidators;
using Domain.Interfaces.AcademicStructureInterfaces.SectionInterfaces;
using Domain.Interfaces.EmployeeInterfaces;
using Application.Services.EmployeeService;
using Contracts.Responses.EmployeeResponse;
using Application.Validators.EmployeeValidator;
using Contracts.Common;
using Contracts.Requests.EmployeeRequests.UniversityAdminRequests;
using Contracts.Requests.EmployeeRequests.CollegeAdminRequests;
using Contracts.Requests.EmployeeRequests.DepartmentAdminRequests;

namespace Application.Extensions
{
    public static class AddServicesToDIContainer
    {
        public static IServiceCollection ServicesToDI(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<IUniversityService, UniversityService>();
            services.AddScoped<ICollegeService, CollegeService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IBatchService, BatchService>();
            services.AddScoped<ISectionService, SectionService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            return services;
        }
        public static IServiceCollection Validators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
            services.AddScoped<IValidator<AddUserDTO>, AddUserValidator>();
            services.AddScoped<IValidator<UpdateUserDTO>, UpdateUserValidator>();
            services.AddScoped<IValidator<AddRoleDTO>, AddRoleValidator>();
            services.AddScoped<IValidator<AddUniversityDTO>, AddUniversityValidator>();
            services.AddScoped<IValidator<UpdateUniversityDTO>, UpdateUniversityValidator>();
            services.AddScoped<IValidator<AddCollegeDTO>, AddCollegeValidator>();
            services.AddScoped<IValidator<UpdateCollegeDTO>, UpdateCollegeValidator>();
            services.AddScoped<IValidator<AddDepartmentDTO>, AddDepartmentValidator>();
            services.AddScoped<IValidator<UpdateDepartmentDTO>, UpdateDepartmentValidator>();
            services.AddScoped<IValidator<AddBatchDTO>, AddBatchValidator>();
            services.AddScoped<IValidator<UpdateBatchDTO>, UpdateBatchValidator>();
            services.AddScoped<IValidator<AddSectionDTO>, AddSectionValidator>();
            services.AddScoped<IValidator<UpdateSectionDTO>, UpdateSectionValidator>();
            //Employee Validators
            services.AddScoped(typeof(IValidator<>), typeof(EmployeeValidator<>));
            services.AddScoped(typeof(IValidator<>), typeof(UpdateEmployeeValidator<>));

            return services;
        }
    }
}
