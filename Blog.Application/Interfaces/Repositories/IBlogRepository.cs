using Blog.Application.Dtos;
using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Repositories
{
    public interface IBlogRepository
    {
        Task<Post> GetByIdAsync(string id);

        Task<Post> CreateAsync(PostDto post);
    }
}