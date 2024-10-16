using Blog.Application.Dtos;
using Blog.Domain.Common;
using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Services
{
    public interface IPostService
    {
        Task<PaginatedResponse<PostDto>> SearchWithPaginatedResponseAsync(int pageNumber = 1, int pageSize = 10, Func<IQueryable<Post>, IQueryable<Post>>? predicate = null);

        Task<PostDto> GetByIdAsync(string id);

        Task<PostDto> CreateAsync(PostDto post);

        Task<PostDto> UpdateAsync(PostDto post);

        Task<bool> DeleteAsync(string id);
    }
}