using Blog.Application.Dtos.Category;
using Blog.Domain.Common;
using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<PaginatedResponse<CategoryDto>> SearchWithPaginatedResponseAsync(int pageNumber = 1, int pageSize = 10, Func<IQueryable<Post>, IQueryable<Post>>? predicate = null);

        Task<CategoryDto> GetByIdAsync(string id);

        Task<CategoryDto> CreateAsync(CategoryCreateRequest request);

        Task<CategoryDto> UpdateAsync(CategoryUpdateRequest request);

        Task<bool> DeleteAsync(string id);
    }
}