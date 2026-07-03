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

namespace Application.Extensions
{
    public static class AddServicesToDIContainer
    {
        public static IServiceCollection ServicesToDI(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            return services;
        }
        public static IServiceCollection Validators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
            return services;
        }
    }
}
