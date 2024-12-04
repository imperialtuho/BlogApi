using Blog.Application.Configurations.MappingProfiles.AutoMapper;
using Blog.Application.Configurations.MappingProfiles.Mapster;
using Blog.Application.Configurations.Settings;
using Blog.Application.Interfaces.Services;
using Blog.Application.Services;
using Blog.Domain.Constants;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
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

            const int MaxRequestBodySize = 100000000;
            string _myAllowSpecificOrigins = ApplicationConstants.MyAllowSpecificOrigins;
            // Adds system services
            services.AddHttpClient();

            // Dependency injection support for Mapster
            // https://github.com/MapsterMapper/Mapster/wiki/Dependency-Injection
            var config = TypeAdapterConfig.GlobalSettings;
            config.Apply(new MappingRegistration());
            config.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            // Memory cache.
            services.AddMemoryCache();

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy(_myAllowSpecificOrigins,
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = MaxRequestBodySize;
            });

            // Adds application services

            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICategoryService, CategoryService>();

            return services;
        }
    }
}