using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Contracts.DTOs.Login_Request_DTO;
using Contracts.Validators.LoginRequestValidator;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;





namespace Contracts.Extensions
{
    public static class AddServicesToDIExtensions
    {
        //public static IServiceCollection Applications()



        public static IServiceCollection Validators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<LoginRequestDTO>, LoginRequestValidator>();
            return services;
        }
    }
}
