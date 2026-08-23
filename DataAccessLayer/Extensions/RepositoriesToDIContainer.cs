using DataAccessLayer.Repos;
using DataAccessLayer.Repos.AcademicRepositories;
using DataAccessLayer.Repos.EmployeeRepository;
using DataAccessLayer.Repos.IdentityRepositories;
using DataAccessLayer.Repos.StudentRepositories;
using DataAccessLayer.Repos.StudentRepository;
using Domain.Interfaces.AcademicStructureInterfaces.BatchInterfaces;
using Domain.Interfaces.AcademicStructureInterfaces.CollegeInterfaces;
using Domain.Interfaces.AcademicStructureInterfaces.DepartmentInterfaces;
using Domain.Interfaces.AcademicStructureInterfaces.SectionInterfaces;
using Domain.Interfaces.AcademicStructureInterfaces.UniversityInterfaces;
using Domain.Interfaces.EmployeeInterfaces;
using Domain.Interfaces.IdentityInterfaces.RoleInterfaces;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Domain.Interfaces.IdentityInterfaces.UserRoleInterfaces;
using Domain.Interfaces.LoginInterfaces.TokenInterfaces;
using Domain.Interfaces.StudentInterfaces;
using Domain.Interfaces.StudentStatusInterfaces;
using Domain.Interfaces.StudyInterfaces.SubjectInterfaces;
using Domain.Interfaces.UnitOfWork;
using DataAccessLayer.Repos.StudyRepository;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Extensions
{
    public static class RepositoriesToDIContainer
    {
        public static IServiceCollection  AddRepoSitoriesToDIContainer(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWorkRepository, UnitOfWorkRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IUniversityRepository,UniversityRepository>();
            services.AddScoped<ICollegeRepository, CollegeRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IBatchRepository, BatchRepository>();
            services.AddScoped<ISectionRepository, SectionRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IStatusRepository, StudentStatusRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            return services;
        }
    }
}
