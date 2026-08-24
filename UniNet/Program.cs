
using Application.Extensions;
using DataAccessLayer.Configurations.Options;
using DataAccessLayer.Dbcontext;
using DataAccessLayer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using UniNet.MiddleWares;
using Microsoft.AspNetCore.Authorization;
using UniNet.Authorization.AuthorizationRequirements;
using DataAccessLayer.Seeds;
using UniNet.Authorization.AuthorizationHandlers.EmployeeHandlers;
using UniNet.Authorization.AuthorizationHandlers.StudentHandler;
using UniNet.Authorization.AuthorizationHandlers.AcademicAuthorizationHandlers;
using UniNet.Extensions;

namespace UniNet
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //JWT Options
            builder.Services.Configure<JWTOption>(builder.Configuration.GetSection("Jwt"));
            var JwtSection = builder.Configuration.GetSection("Jwt");
            var JwtOptions = JwtSection.Get<JWTOption>() ??
                throw new InvalidOperationException("Jwt configuration section is missing. Set Jwt:Key/Issuer/Audience in user-secrets.");
            //Auth
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // TokenValidationParameters define how incoming JWTs will be validated.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Ensures the token was issued by a trusted issuer.
            ValidateIssuer = true,


            // Ensures the token is intended for this API (audience check).
            ValidateAudience = true,


            // Ensures the token has not expired.
            ValidateLifetime = true,


            // Ensures the token signature is valid and was signed by the API.
            ValidateIssuerSigningKey = true,


            // The expected issuer value (must match the issuer used when creating the JWT).
            ValidIssuer = JwtOptions!.Issuer,


            // The expected audience value (must match the audience used when creating the JWT).
            ValidAudience = JwtOptions.Audience,


            // The secret key used to validate the JWT signature.
            // This must be the same key used when generating the token.
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(JwtOptions.Key))
        };
        options.Events = new JwtBearerEvents
        {
            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new
                {
                    status = 403,
                    message = "Access denied: You do not have the required role."
                });

                return context.Response.WriteAsync(result);
            },

            OnChallenge = context =>
            {
                context.HandleResponse();

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new
                {
                    status = 401,
                    message = "Unauthorized: Please login first."
                });

                return context.Response.WriteAsync(result);
            }
        };
    });
            // Add services to the container.
            builder.Services.AddDbContext<AppDbcontext>(options =>
options.UseSqlServer(
builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddControllers();

            //Validators Services
            builder.Services.Validators();


            // Application Services
            builder.Services.ServicesToDI();

            
            //Repositories To DI
            builder.Services.AddRepoSitoriesToDIContainer();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(options =>
            {
                // 1. Define the security scheme for JWT Bearer
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter your JWT token in this format: Bearer {your_token}"
                });

                // 2. Apply this security requirement globally to all endpoints
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
            });


            //Add Policyes 
            builder.Services.AddPolicyToDIContainer();

            // Add authorizationHandlers to the DI container
            builder.Services.AuthorizationHandlersToDIContainer();

            // Add HttpContextAccessor to the DI container
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("UniNetPolicy", policy =>
                {
                    policy
                        .WithOrigins(
                            "https://localhost:7082",
                            "http://localhost:5126",
                            "http://localhost:5500",
                            "http://127.0.0.1:5500",
                            "https://localhost:7175", // UniNet.Client (WASM) — HTTPS profile
                            "http://localhost:5286"   // UniNet.Client (WASM) — HTTP profile
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                using var scop = app.Services.CreateScope();
                var db = scop.ServiceProvider.GetRequiredService<AppDbcontext>();
                await db.Database.MigrateAsync();
                await TestDataSeeder.SeedAsync(db);
                app.UseSwagger();
                app.UseSwaggerUI();
            }

    


            app.UseHttpsRedirection();
            // Serve the frontend (wwwroot/index.html) from the same host
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCors("UniNetPolicy");
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
