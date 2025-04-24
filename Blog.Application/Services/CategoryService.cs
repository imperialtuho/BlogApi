using AutoMapper;
using Blog.Application.Dtos.Category;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;
using Blog.Domain.Common;
using Blog.Domain.Entities;
using Blog.Domain.Exceptions;
using Blog.Domain.Helpers;
using Microsoft.AspNetCore.Http;

namespace Blog.Application.Services
{
    /// <summary>
    /// The category service.
    /// </summary>
    /// <param name="categoryRepository">The categoryRepository.</param>
    /// <param name="httpContextAccessor">The httpContextAccessor.</param>
    /// <param name="mapper">The mapper.</param>
    public class CategoryService(ICategoryRepository categoryRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper) : BaseService(httpContextAccessor, mapper), ICategoryService
    {
        public async Task<CategoryDto> CreateAsync(CategoryCreateRequest request)
        {
            if (request == null)
            {
                throw new InvalidOperationException($"{nameof(request)} cannot be null.");
            }

            var caterogy = new Category()
            {
                Title = request.Title,
                Label = request.Label,
                Description = request.Description,
                DisplayPosition = request.DisplayPosition,
                AuthorId = LoginSession?.UserId ?? throw new ForbiddenException($"You don't have permission to do this action!"),
                CoverImageUrl = request.ImageUrl
            };

            Category newCategory = await categoryRepository.AddWithSaveChangesAndReturnModelAsync(caterogy);

            return _mapper.Map<CategoryDto>(newCategory);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            Category category = await categoryRepository.GetByIdAsync(id);

            CheckingCurrentPerformingOperation(category.AuthorId, category.TenantId);

            return await categoryRepository.ForceDeleteAsync(category);
        }

        public async Task<CategoryDto> GetByIdAsync(string id)
        {
            Category category = await categoryRepository.GetWithRelationByIdAsync(id);

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<PaginatedResponse<CategoryDto>> SearchAsync(SearchRequest request)
        {
            FilterBuildingHelper<Category>? filterBuilder = new(request.Filters ?? []);
            Func<IQueryable<Category>, IQueryable<Category>>? filterExpression = filterBuilder.Build();

            PaginatedResponse<Category> result = await categoryRepository.SearchWithPaginatedResponseAsync(request.PageNumber, request.PageSize, filterExpression);

            return new PaginatedResponse<CategoryDto>(_mapper.Map<IReadOnlyCollection<CategoryDto>>(result.Data), result.TotalCount, result.PageNumber, result.TotalPages);
        }

        public async Task<CategoryDto> UpdateAsync(CategoryUpdateRequest request)
        {
            Category existCategory = await categoryRepository.GetByIdAsync(request.Id) ?? throw new NotFoundException($"{nameof(Category)} with provided id:{request.Id} could not be found.");

            _mapper.Map(request, existCategory);

            CheckingCurrentPerformingOperation(existCategory.AuthorId, existCategory.TenantId);

            return _mapper.Map<CategoryDto>(await categoryRepository.UpdateWithSaveChangesAndReturnModelAsync(existCategory));
        }
    }
}