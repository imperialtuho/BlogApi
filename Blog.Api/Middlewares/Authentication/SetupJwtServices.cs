﻿using Blog.Application.Configurations.Settings;
using Microsoft.IdentityModel.Logging;

namespace Blog.Api.Middlewares.Authentication
{
    public static class SetupJwtServices
    {
        public static void AddJwtServices(this IServiceCollection services, IConfiguration configuration)
        {
            AuthenticationMiddlewareHandler.IdentityUrl = configuration.GetValue<string>($"{nameof(JwtSettings)}:IdentityUrl");

            services.AddAuthentication("Basic")
                .AddScheme<AuthenticationMiddlewareOptions, AuthenticationMiddlewareHandler>("Basic", op => { });
        }
    }
}