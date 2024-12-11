using Blog.Application.Configurations.Database;
using Blog.Application.Interfaces.ExternalProviders;
using Blog.Application.Interfaces.Repositories;
using Blog.Infrastructure.Configurations;
using Blog.Infrastructure.Database;
using Blog.Infrastructure.Repositories.ExternalProviders.AssetApi;
using Blog.Infrastructure.Repositories.ExternalProviders.IdentityApi;
using Blog.Infrastructure.Repositories.Providers.Blogs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Principal;

namespace Blog.Infrastructure
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds Infrastructure Services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>IServiceCollection.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();

            // Adds HTTP client
            services.AddHttpContextAccessor();
            services.AddTransient<IPrincipal>(provider => provider.GetService<IHttpContextAccessor>()!.HttpContext!.User);
            services.AddHttpClient();

            // Memory cache.
            services.AddMemoryCache();

            // Adds API client services
            services.AddTransient<IIdentityApi, IdentityApi>();
            services.AddTransient<IAssetApi, AssetApi>();

            // Adds SqlConnectionFactory
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            // Register DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            // Adds Repositories.
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IMediaRepository, MediaRepository>();

            return services;
        }
    }
}