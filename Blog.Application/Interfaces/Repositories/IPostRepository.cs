using Blog.Application.Dtos;
using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Repositories
{
    public interface IPostRepository
    {
        Task<Post> GetByIdAsync(string id);

        Task<Post> CreateAsync(PostDto post);

        Task<Post> UpdateAsync(PostDto post);

        Task DeleteAsync(string id);
    }
}