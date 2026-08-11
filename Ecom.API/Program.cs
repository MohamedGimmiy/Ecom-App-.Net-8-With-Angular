
using Ecom.Infrastructure;
using Microsoft.Extensions.FileProviders;
namespace Ecom.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddCors(
                options =>
                {
                    options.AddPolicy("CORSPolicy", policy =>
                    {
                        policy
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials()
                              .WithOrigins("http://localhost:4200");
                    });
                }
                );
            // Add services to the container.
            builder.Services.AddMemoryCache();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            builder.Services.infrastructureConfiguration(builder.Configuration);
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
            });

            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();
            app.UseMiddleware<Middleware.ExceptionMiddleware>();
            // errors page middleware
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors("CORSPolicy");

            app.MapControllers();

            app.Run();
        }
    }
}
