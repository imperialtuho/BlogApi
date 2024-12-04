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
using System.Collections.Immutable;

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

            AuthorDto? author = await identityApi.GetUserByIdAsync(LoginSession!.UserId) ?? throw new InvalidOperationException($"Invalid AuthorId: {LoginSession.UserId}, the user with provided id could not be found!");

            var caterogy = new Category()
            {
                Title = request.Title,
                Label = request.Label,
                Description = request.Description,
                Slug = StringHelper.ToSlug(request.Title),
                DisplayPosition = request.DisplayPosition,
                UserId = LoginSession!.UserId
            };

            if (request.Media != null && request.Media.Count > 0)
            {
                caterogy.Media = request.Media.Adapt<ICollection<Media>>();
            }

            Category newCategory = await categoryRepository.AddWithSaveChangesAndReturnModelAsync(caterogy);

            var result = newCategory.Adapt<CategoryDto>();
            result.Author = author;

            return result;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            Category category = await categoryRepository.GetEntityByIdAsync(id);

            if (IsActionPerformByAdmin(LoginSession) || (!string.IsNullOrEmpty(category.UserId) && (LoginSession?.UserId.Equals(category.UserId) ?? false)))
            {
                return await categoryRepository.DeleteAndSaveChangesAsync(category);
            }

            throw new ForbiddenException($"You're not allowed to update this {nameof(Category)}, reason: {nameof(Category)} is not belong to current user");
        }

        public async Task<CategoryDto> GetByIdAsync(string id)
        {
            Category category = await categoryRepository.GetEntityWithRelationByIdAsync(id);
            AuthorDto? author = await identityApi.GetUserByIdAsync(category.UserId) ?? throw new InvalidOperationException($"Invalid AuthorId: {category.UserId}, the user with provided id could not be found!");

            var result = category.Adapt<CategoryDto>();
            result.Author = author;

            return result;
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

            PaginatedResponse<Category> categories = await categoryRepository.SearchWithPaginatedResponseAsync(request.PageNumber, request.PageSize, predicate);

            IList<AuthorDto>? authors = await identityApi.GetUserByIdsAsync(categories.Data.Select(cat => cat.UserId).ToList()) ?? throw new InvalidOperationException("Couldn't be found any Authors following found categories");

            ImmutableList<CategoryDto> result = categories.Data.AsEnumerable().Select(category =>
            {
                var dto = category.Adapt<CategoryDto>();
                dto.Author = authors.First(a => a.Id.Equals(category.UserId));

                return dto;
            }).ToImmutableList();

            return new PaginatedResponse<CategoryDto>(result, categories.TotalCount, categories.PageNumber, categories.TotalPages);
        }

        public async Task<CategoryDto> UpdateAsync(CategoryUpdateRequest request)
        {
            Category existCategory = await categoryRepository.GetEntityByIdAsync(request.Id) ?? throw new NotFoundException($"{nameof(Category)} with provided id:{request.Id} could not be found.");

            existCategory = request.Adapt(existCategory);

            if (IsActionPerformByAdmin(LoginSession) || (!string.IsNullOrEmpty(existCategory.UserId) && (LoginSession?.UserId.Equals(existCategory.UserId) ?? false)))
            {
                AuthorDto? author = await identityApi.GetUserByIdAsync(existCategory.UserId) ?? throw new InvalidOperationException($"Author is not found by id {existCategory.UserId}");
                CategoryDto result = (await categoryRepository.UpdateWithSaveChangesAndReturnModelAsync(existCategory)).Adapt<CategoryDto>();
                result.Author = author;

                return result;
            }

            throw new ForbiddenException($"You're not allowed to update this {nameof(Category)}, reason: {nameof(Category)} is not belong to current user");
        }
    }
}