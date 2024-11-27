using Blog.Application.Dtos.Author;

namespace Blog.Application.Interfaces.ExternalProviders
{
    public interface IIdentityApi
    {
        Task<AuthorDto?> GetUserByIdAsync(string id);
    }
}