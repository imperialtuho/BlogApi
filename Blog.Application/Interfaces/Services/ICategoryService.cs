using Blog.Application.Dtos.Category;
using Blog.Domain.Common;

namespace Blog.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<PaginatedResponse<CategoryDto>> SearchAsync(SearchRequest request);

        Task<CategoryDto> GetByIdAsync(string id);

        Task<CategoryDto> CreateAsync(CategoryCreateRequest request);

        Task<CategoryDto> UpdateAsync(CategoryUpdateRequest request);

        Task<bool> DeleteAsync(string id);
    }
}