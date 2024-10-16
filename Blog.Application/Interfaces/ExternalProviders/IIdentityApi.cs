using Blog.Application.Dtos;

namespace Blog.Application.Interfaces.ExternalProviders
{
    public interface IIdentityApi
    {
        Task<UserDto> GetUserByIdAsync(string id);
    }
}