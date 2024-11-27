using Blog.Application.Dtos.Category;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;
using Blog.Domain.Common;
using Blog.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Blog.Application.Services
{
    public class CategoryService(ICategoryRepository categoryRepository, IHttpContextAccessor httpContextAccessor) : BaseService(httpContextAccessor), ICategoryService
    {
        public Task<CategoryDto> CreateAsync(CategoryCreateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<CategoryDto> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResponse<CategoryDto>> SearchWithPaginatedResponseAsync(int pageNumber = 1, int pageSize = 10, Func<IQueryable<Post>, IQueryable<Post>>? predicate = null)
        {
            throw new NotImplementedException();
        }

        public Task<CategoryDto> UpdateAsync(CategoryUpdateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}