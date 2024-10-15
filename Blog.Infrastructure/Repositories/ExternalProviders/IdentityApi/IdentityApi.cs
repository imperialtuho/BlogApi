using Blog.Application.Configurations.Settings;
using Blog.Application.Dtos;
using Blog.Application.Interfaces.ExternalProviders;
using Blog.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Blog.Infrastructure.Repositories.ExternalProviders.IdentityApi
{
    public class IdentityApi : WebApiClient, IIdentityApi
    {
        private readonly IdentityApiSettings _identityApiSettings;
        private readonly ILogger<IdentityApi> _logger;

        public IdentityApi(ILogger<IdentityApi> logger, HttpClient httpClient, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IOptions<IdentityApiSettings> identityApiSettings) : base(logger, httpClient, httpClientFactory, httpContextAccessor)
        {
            _identityApiSettings = identityApiSettings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Get user by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{UserDto}.</returns>
        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            string message = $"Call to {nameof(GetUserByIdAsync)}. with id: {id}";
            _logger.LogInformation(message);

            try
            {
                string providerName = _identityApiSettings.ProviderName;
                string requestUri = $"{_identityApiSettings.Url}Users/{id}";

                FluentUriBuilder request = CreateRequest(requestUri);

                var response = await GetAsync<UserDto>(
                    $"{nameof(GetUserByIdAsync)} {requestUri}",
                    request.Uri,
                    providerName,
                    CancellationToken.None,
                    listRequestHeader: AddDefaultAccessTokenRequestHeaders());

                return response ?? new UserDto();
            }
            catch (Exception ex)
            {
                message = $"{nameof(GetUserByIdAsync)} Unknown error encountered";
                _logger.LogError(ex, message);

                throw new UnhandledException(ex.Message);
            }
        }
    }
}