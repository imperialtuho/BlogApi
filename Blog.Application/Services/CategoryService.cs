using Blog.Application.Dtos.Category;
using Blog.Application.Dtos.Post;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;
using Blog.Domain.Common;
using Blog.Domain.Entities;

namespace Blog.Application.Services
{
    public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
    {
        public Task<PostDto> CreateAsync(CategoryCreateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<PostDto> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResponse<CategoryDto>> SearchWithPaginatedResponseAsync(int pageNumber = 1, int pageSize = 10, Func<IQueryable<Post>, IQueryable<Post>>? predicate = null)
        {
            throw new NotImplementedException();
        }

        public Task<PostDto> UpdateAsync(CategoryUpdateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}