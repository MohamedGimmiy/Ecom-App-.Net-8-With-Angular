using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Infrastructure.Data;
using Ecom.Infrastructure.Repositories;
using Ecom.Infrastructure.Repositories.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

namespace Ecom.Infrastructure
{
    public static class InfrastructureRegisteration
    {
        public static IServiceCollection infrastructureConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            //services.AddScoped(typeof(IGenericRepositry<>), typeof(GenericRepository<>));
            //services.AddScoped<ICategoryRepositry, CategoryRepositry>();
            //services.AddScoped<IProductRepositry, ProductRepositry>();
            //services.AddScoped<IPhotoRepositry, PhotoRepositry>();

            services.AddSingleton<IFileProvider>( new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
            );
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // register email sender
            services.AddScoped<IEmailService, EmailService>();
            // register token
            services.AddScoped<IGenerateToken, GenerateToken>();
            // Apply redis connecting
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var conf = ConfigurationOptions.Parse(configuration.GetConnectionString("redis"), true);
                return ConnectionMultiplexer.Connect(conf);
            });
            services.AddSingleton<IImageManagementService, ImageManagementService>();


            services.AddDbContext<AppDbContext>(op =>
            {
                op.UseSqlServer(configuration.GetConnectionString("EcomDatabase"));
            });

            services.AddAuthentication(op =>
            {
                op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                op.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            }).AddCookie(op =>
            {
                op.Cookie.Name = "cookie";
                op.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
            }).AddJwtBearer(op => {
                op.RequireHttpsMetadata = false;
                op.SaveToken = true;
                op.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Secret"])),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Token:Issuer"],
                    ValidateAudience= false,
                    ClockSkew = TimeSpan.Zero
                };
                op.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["token"];
                        return Task.CompletedTask;
                    }
                };
            });
            return services;
        }
    }
}
