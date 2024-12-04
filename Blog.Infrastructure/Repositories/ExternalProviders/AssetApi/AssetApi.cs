using Blog.Application.Dtos.Media;
using Blog.Application.Interfaces.ExternalProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Blog.Infrastructure.Repositories.ExternalProviders.AssetApi
{
    public class AssetApi(ILogger<WebApiClient> logger,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor) : WebApiClient(logger, httpClientFactory, httpContextAccessor), IAssetApi
    {
        public Task<IList<MediaDto>> GetMediaInformationByIdsAsync(IEnumerable<string> mediaIds)
        {
            throw new NotImplementedException();
        }
    }
}