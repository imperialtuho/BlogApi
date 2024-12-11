using Blog.Application.Configurations.MappingProfiles.AutoMapper;
using Blog.Application.Configurations.MappingProfiles.Mapster;
using Blog.Application.Configurations.Settings;
using Blog.Application.Interfaces.Services;
using Blog.Application.Services;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Blog.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Adds setting json
            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
            services.Configure<ApplicationSettings>(configuration.GetSection(nameof(ApplicationSettings)));
            services.Configure<IdentityApiSettings>(configuration.GetSection(nameof(IdentityApiSettings)));

            // Dependency injection support for Mapster
            // https://github.com/MapsterMapper/Mapster/wiki/Dependency-Injection
            var config = TypeAdapterConfig.GlobalSettings;
            config.Apply(new MappingRegistration());
            config.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

            // Adds application services
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICategoryService, CategoryService>();

            return services;
        }
    }
}