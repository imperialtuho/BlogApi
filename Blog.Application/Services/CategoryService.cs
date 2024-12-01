using Blog.Application.Dtos.Category;
using Blog.Application.Interfaces.ExternalProviders;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;
using Blog.Domain.Common;
using Blog.Domain.Entities;
using Blog.Domain.Exceptions;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace Blog.Application.Services
{
    public class CategoryService(ICategoryRepository categoryRepository, IIdentityApi identityApi, IHttpContextAccessor httpContextAccessor) : BaseService(httpContextAccessor), ICategoryService
    {
        public async Task<CategoryDto> CreateAsync(CategoryCreateRequest request)
        {
            if (request == null)
            {
                throw new InvalidOperationException($"{nameof(request)} cannot be null.");
            }

            if (!string.IsNullOrEmpty(request.UserId))
            {
                // Validates User before creating Post.
                _ = await identityApi.GetUserByIdAsync(request.UserId) ?? throw new InvalidOperationException($"Invalid User Id: {request.UserId}, the user with provided id could not be found!");
            }

            var caterogy = request.Adapt<Category>();

            Category newPost = await categoryRepository.AddWithSaveChangesAndReturnModelAsync(caterogy);

            return newPost.Adapt<CategoryDto>();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            Category category = await categoryRepository.GetEntityByIdAsync(id);

            if (IsActionPerformByAdmin(LoginSession) || (!category.IsGlobal && !string.IsNullOrEmpty(category.UserId) && (LoginSession?.UserId.Equals(category.UserId) ?? false)))
            {
                return await categoryRepository.DeleteAndSaveChangesAsync(category);
            }

            throw new ForbiddenException($"You're not allowed to update this {nameof(Category)}, reason: {nameof(Category)} is not belong to current user");
        }

        public async Task<CategoryDto> GetByIdAsync(string id)
        {
            Category category = await categoryRepository.GetEntityWithRelationByIdAsync(id);

            return category.Adapt<CategoryDto>();
        }

        public async Task<PaginatedResponse<CategoryDto>> SearchAsync(SearchRequest request)
        {
            IQueryable<Category> predicate(IQueryable<Category> category) => category.Where(x => x.Name.Contains(request.Keyword));
            PaginatedResponse<Category> result = await categoryRepository.SearchWithPaginatedResponseAsync(request.PageNumber, request.PageSize, predicate);

            return new PaginatedResponse<CategoryDto>(result.Items.Adapt<IReadOnlyCollection<CategoryDto>>(), result.TotalCount, result.PageNumber, result.TotalPages);
        }

        public async Task<CategoryDto> UpdateAsync(CategoryUpdateRequest request)
        {
            Category category = request.Adapt<Category>();

            if (IsActionPerformByAdmin(LoginSession) || (!category.IsGlobal && !string.IsNullOrEmpty(category.UserId) && (LoginSession?.UserId.Equals(category.UserId) ?? false)))
            {
                return (await categoryRepository.UpdateWithSaveChangesAndReturnModelAsync(category)).Adapt<CategoryDto>();
            }

            throw new ForbiddenException($"You're not allowed to update this {nameof(Category)}, reason: {nameof(Category)} is not belong to current user");
        }
    }
}