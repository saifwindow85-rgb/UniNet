using Application.Services.IdentityServices;
using Domain.Interfaces.LoginInterfaces;
using Domain.Interfaces.UnitOfWork;
using Domain.Interfaces.UserInterfaces;
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

namespace Application.Extensions
{
    public static class AddServicesToDIContainer
    {
        public static IServiceCollection ServicesToDI(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            return services;
        }
        public static IServiceCollection Validators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
            services.AddScoped<IValidator<AddUserDTO>, AddUserValidator>();
            services.AddScoped<IValidator<UpdateUserDTO>, UpdateUserValidator>();
            return services;
        }
    }
}
