using Blog.Application.Dtos.Category;
using Blog.Application.Dtos.Post;
using Blog.Domain.Common;
using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<PaginatedResponse<CategoryDto>> SearchWithPaginatedResponseAsync(int pageNumber = 1, int pageSize = 10, Func<IQueryable<Post>, IQueryable<Post>>? predicate = null);

        Task<PostDto> GetByIdAsync(string id);

        Task<PostDto> CreateAsync(CategoryCreateRequest request);

        Task<PostDto> UpdateAsync(CategoryUpdateRequest request);

        Task<bool> DeleteAsync(string id);
    }
}