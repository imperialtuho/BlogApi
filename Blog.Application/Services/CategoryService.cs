using Blog.Application.Dtos.Author;
using Blog.Application.Dtos.Category;
using Blog.Application.Interfaces.ExternalProviders;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;
using Blog.Domain.Common;
using Blog.Domain.Entities;
using Blog.Domain.Exceptions;
using Blog.Domain.Extensions;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace Blog.Application.Services
{
    public class CategoryService(ICategoryRepository categoryRepository, IPostRepository postRepository, IIdentityApi identityApi, IHttpContextAccessor httpContextAccessor) : BaseService(httpContextAccessor), ICategoryService
    {
        public async Task<CategoryDto> CreateAsync(CategoryCreateRequest request)
        {
            if (request == null)
            {
                throw new InvalidOperationException($"{nameof(request)} cannot be null.");
            }

            AuthorDto? author;
            author = await identityApi.GetUserByIdAsync(LoginSession!.UserId.ToString()) ?? throw new InvalidOperationException($"Invalid AuthorId: {LoginSession.UserId}, the user with provided id could not be found!");

            var caterogy = new Category()
            {
                Title = request.Title,
                Label = request.Label,
                Description = request.Description,
                CoverImageUrl = request.CoverImageUrl,
                Slug = StringHelper.ToSlug(request.Title),
                DisplayPosition = request.DisplayPosition,
                UserId = LoginSession!.UserId.ToString()
            };

            Category newCategory = await categoryRepository.AddWithSaveChangesAndReturnModelAsync(caterogy);

            var result = newCategory.Adapt<CategoryDto>();
            result.Author = author;

            return result;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            Category category = await categoryRepository.GetEntityByIdAsync(id);

            if (IsActionPerformByAdmin(LoginSession) || (!string.IsNullOrEmpty(category.CreatedBy) && (LoginSession?.UserId.Equals(category.CreatedBy) ?? false)))
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
            Func<IQueryable<Category>, IQueryable<Category>> predicate = categories =>
            {
                // Base filter: match keyword in Name
                IQueryable<Category> query = categories.Where(entity => entity.Label.Contains(request.Keyword));

                // Filter out deleted categories if not including deleted
                if (!request.IsIncludingDelete)
                {
                    query = query.Where(entity => !entity.IsDeleted);
                }

                return query;
            };

            PaginatedResponse<Category> result = await categoryRepository.SearchWithPaginatedResponseAsync(request.PageNumber, request.PageSize, predicate);

            return new PaginatedResponse<CategoryDto>(result.Items.Adapt<IReadOnlyCollection<CategoryDto>>(), result.TotalCount, result.PageNumber, result.TotalPages);
        }

        public async Task<CategoryDto> UpdateAsync(CategoryUpdateRequest request)
        {
            Category category = request.Adapt<Category>();

            if (IsActionPerformByAdmin(LoginSession) || (!string.IsNullOrEmpty(category.UserId) && (LoginSession?.UserId.Equals(category.UserId) ?? false)))
            {
                return (await categoryRepository.UpdateWithSaveChangesAndReturnModelAsync(category)).Adapt<CategoryDto>();
            }

            throw new ForbiddenException($"You're not allowed to update this {nameof(Category)}, reason: {nameof(Category)} is not belong to current user");
        }
    }
}