using Blog.Application.Dtos.Media;

namespace Blog.Application.Interfaces.ExternalProviders
{
    public interface IAssetApi
    {
        public Task<IList<MediaDto>> GetMediaInformationByIdsAsync(IEnumerable<string> mediaIds);
    }
}