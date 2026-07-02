
using Contracts.Extensions;
using DataAccessLayer.Dbcontext;
using Microsoft.EntityFrameworkCore;

namespace UniNet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.
            builder.Services.AddDbContext<AppDbcontext>(options =>
options.UseSqlServer(
builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddControllers();
            //Application Services
            builder.Services.Validators();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("UniNetPolicy", policy =>
                {
                    policy
                        .WithOrigins(
                            "https://localhost:7082",
                            "http://localhost:5126"
                        )
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

    


            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
