using Blog.Application.Dtos;

namespace Blog.Application.Interfaces.Services
{
    public interface IPostService
    {
        Task<PostDto> GetByIdAsync(string id);

        Task<PostDto> CreateAsync(PostDto post);

        Task<PostDto> UpdateAsync(PostDto post);

        Task<bool> DeleteAsync(string id);
    }
}