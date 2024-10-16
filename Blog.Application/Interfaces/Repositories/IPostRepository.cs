using Blog.Application.Dtos;
using Blog.Domain.Common;
using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Repositories
{
    public interface IPostRepository
    {
        Task<PaginatedResponse<Post>> SearchWithPaginatedResponseAsync(int pageNumber = 1, int pageSize = 10, Func<IQueryable<Post>, IQueryable<Post>>? predicate = null);

        Task<Post> GetByIdAsync(string id);

        Task<Post> CreateAsync(PostDto post);

        Task<Post> UpdateAsync(PostDto post);

        Task DeleteAsync(string id);
    }
}