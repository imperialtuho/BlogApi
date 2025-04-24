using AutoMapper;
using Blog.Application.Dtos.Post;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;
using Blog.Domain.Common;
using Blog.Domain.Entities;
using Blog.Domain.Enums;
using Blog.Domain.Exceptions;
using Blog.Domain.Helpers;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Blog.Application.Services
{
    /// <summary>
    /// The Post Service.
    /// </summary>
    /// <param name="postRepository">The postRepository.</param>
    /// <param name="categoryRepository">The categoryRepository.</param>
    /// <param name="postCategoryRepository">The postCategoryRepository.</param>
    /// <param name="httpContextAccessor">The httpContextAccessor.</param>
    /// <param name="mapper">The mapper.</param>
    public class PostService(IPostRepository postRepository,
        ICategoryRepository categoryRepository,
        IPostCategoryRepository postCategoryRepository,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper) : BaseService(httpContextAccessor, mapper), IPostService
    {
        private const int PostCategoryLimit = 10;

        public async Task<bool> AssignCategoriesAsync(string id, IList<string> categoryIds)
        {
            if (string.IsNullOrEmpty(id) || (categoryIds == null || categoryIds.Count == 0))
            {
                throw new ArgumentException($"{nameof(id)} or {nameof(categoryIds)} cannot be null");
            }

            Post post = await postRepository.GetByIdAsync(id);
            IList<Category> categories = await categoryRepository.GetByIdsAsync(categoryIds);

            if (categories == null || categories.Count == 0)
            {
                throw new NotFoundException($"There was no {nameof(Category)} found!");
            }

            if (post.PostCategories?.Count(p => categories.Select(c => c.Id).Equals(p.CategoryId)) > PostCategoryLimit)
            {
                throw new InvalidOperationException($"The post is exceeded the limit {PostCategoryLimit} categories");
            }

            CheckingCurrentPerformingOperation(post.AuthorId, post.TenantId);

            return await postCategoryRepository.AssignCategoriesAsync(id, categoryIds);
        }

        public async Task<PostDto> CreateAsync(PostCreateRequest request)
        {
            if (request == null)
            {
                throw new ArgumentException($"{nameof(request)} cannot be null.");
            }

            ValidatePostStatus(request.Status);

            var post = new Post
            {
                Title = request.Title,
                Summary = request.Summary,
                Content = request.Content,
                CoverImageUrl = request.CoverImageUrl,
                Featured = request.Featured,
                Pinned = request.Pinned,
                CommentingEnabled = request.CommentingEnabled,
                Status = request.Status,
                HashTags = request.HashTags,
                AuthorId = LoginSession?.UserId ?? throw new ForbiddenException($"You don't have permission to do this action!")
            };

            Post newPost = await postRepository.AddWithSaveChangesAndReturnModelAsync(post);

            return _mapper.Map<PostDto>(newPost);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            Post post = await postRepository.GetByIdAsync(id);

            List<string>? categoryIds = post.PostCategories?.Select(x => x.CategoryId).ToList();

            CheckingCurrentPerformingOperation(post.AuthorId, post.TenantId);

            if (categoryIds != null && categoryIds.Any())
            {
                await postCategoryRepository.UnAssignCategoriesAsync(post.Id, categoryIds);
            }

            return await postRepository.SoftDeleteAndSaveChangesAsync(post);
        }

        public async Task<PostDto> GetByIdAsync(string id)
        {
            Post post = await postRepository.GetWithRelationByIdAsync(id) ?? throw new NotFoundException($"No {nameof(Post)} was found by id:{id}");

            var result = post.Adapt<PostDto>();

            return result;
        }

        public async Task<IList<PostDto>> GetByStatusAndAuthorIdAsync(string status, string authorId)
        {
            ValidatePostStatus(status);

            IList<Post> posts = await postRepository.GetByStatusAndAuthorIdAsync(status, authorId);

            if (posts == null || posts.Count == 0)
            {
                throw new NotFoundException($"No {nameof(Post)} was found.");
            }

            var result = _mapper.Map<IList<PostDto>>(posts);

            return result;
        }

        public async Task<PaginatedResponse<PostDto>> SearchAsync(SearchRequest request)
        {
            FilterBuildingHelper<Post>? filterBuilder = new(request.Filters ?? []);
            Func<IQueryable<Post>, IQueryable<Post>>? filterExpression = filterBuilder.Build();

            PaginatedResponse<Post> result = await postRepository.SearchWithPaginatedResponseAsync(request.PageNumber, request.PageSize, filterExpression);

            return new PaginatedResponse<PostDto>(_mapper.Map<IReadOnlyCollection<PostDto>>(result.Data), result.TotalCount, result.PageNumber, result.TotalPages);
        }

        public async Task<bool> UnAssignCategoriesAsync(string id, IList<string> categoryIds)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new InvalidOperationException($"{nameof(id)} is null or empty");
            }

            Post post = await postRepository.GetByIdAsync(id) ?? throw new NotFoundException($"The {nameof(Post)} with id: {id} is not found.");

            CheckingCurrentPerformingOperation(post.AuthorId, post.TenantId);

            return await postCategoryRepository.UnAssignCategoriesAsync(id, categoryIds);
        }

        public async Task<PostDto> UpdateAsync(PostUpdateRequest request)
        {
            ValidatePostStatus(request.Status);

            Post post = await postRepository.GetByIdAsync(request.Id) ?? throw new NotFoundException($"{nameof(Post)} with id: {request.Id} is not found.");

            CheckingCurrentPerformingOperation(post.AuthorId, post.TenantId);

            return _mapper.Map<PostDto>(await postRepository.UpdateWithSaveChangesAndReturnModelAsync(post));
        }

        public async Task<bool> UpdateSatusAsync(string id, string status)
        {
            ValidatePostStatus(status);

            Post post = await postRepository.GetByIdAsync(id) ?? throw new NotFoundException($"No {nameof(Post)} was found!");

            CheckingCurrentPerformingOperation(post.Id, post.TenantId);

            if (post.Status.Equals(nameof(PostStatus.InReview)))
            {
                throw new InvalidOperationException($"The {nameof(Post)} is currently in review, contact admin for approval");
            }

            post.Status = status;

            return await postRepository.UpdateAndSaveChangesAsync(post);
        }

        private static void ValidatePostStatus(string status)
        {
            if (!Enum.TryParse<PostStatus>(status, ignoreCase: true, out _))
            {
                throw new ArgumentException($"Invalid post status: {status}");
            }
        }
    }
}