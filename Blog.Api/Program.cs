using Blog.Api.Middlewares.Authentication;
using Blog.Application;
using Blog.Application.Configurations.Settings;
using Blog.Domain.Constants;
using Blog.Infrastructure;
using Blog.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;

namespace Blog.Api
{
    public class Program
    {
        protected Program()
        { }

        protected static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string _environmentName = Environment.GetEnvironmentVariable(ApplicationConstants.AspNetCoreEnvironment) ?? ApplicationConstants.DefaultEnvironmentName;

            // Create a logger
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

            logger.LogInformation("Environment name: {_environmentName}", _environmentName);

            // Adds environment variables from json
            builder.Configuration.SetBasePath(builder.Environment.ContentRootPath).AddEnvironmentVariables();

            // Add services to the container.
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddJwtServices(builder.Configuration);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                string? assemblyName = typeof(Program).Assembly.GetName().Name;
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(assemblyName));
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            var appSettings = app.Services.GetRequiredService<IOptions<ApplicationSettings>>().Value;

            // Configure the HTTP request pipeline.
            if (!appSettings.IsProductionMode)
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