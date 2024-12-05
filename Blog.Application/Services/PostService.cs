using Blog.Application.Dtos.Author;
using Blog.Application.Dtos.Media;
using Blog.Application.Dtos.Post;
using Blog.Application.Interfaces.ExternalProviders;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;
using Blog.Domain.Common;
using Blog.Domain.Entities;
using Blog.Domain.Enums;
using Blog.Domain.Exceptions;
using Blog.Domain.Extensions;
using Blog.Domain.Helpers;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace Blog.Application.Services
{
    /// <summary>
    /// The Post Service.
    /// </summary>
    /// <param name="postRepository">The postRepository.</param>
    /// <param name="identityApi">The identityApi.</param>
    public class PostService(IPostRepository postRepository,
        ICategoryRepository categoryRepository,
        IMediaRepository mediaRepository,
        IIdentityApi identityApi,
        IAssetApi assetApi,
        IHttpContextAccessor httpContextAccessor) : BaseService(httpContextAccessor), IPostService
    {
        private const int CategoryLimit = 10;

        public async Task<IList<string>> AssignCategoryToPostAsync(string id, IList<string> categoryIds)
        {
            if (string.IsNullOrEmpty(id) || (categoryIds == null || categoryIds.Count == 0))
            {
                throw new ArgumentException($"{nameof(id)} or {nameof(categoryIds)} cannot be null");
            }

            Post post = await postRepository.GetEntityByIdAsync(id);
            IList<Category> categories = await categoryRepository.GetByIdsAsync(categoryIds);

            if (post.PostCategories?.Count(p => categories.Select(c => c.Id).Equals(p.CategoryId)) > CategoryLimit)
            {
                throw new InvalidOperationException($"The post is exceeded the limit {CategoryLimit} categories");
            }

            if (categories == null || categories.Count == 0)
            {
                throw new NotFoundException($"There was no {nameof(Category)} found!");
            }

            if (!IsCurrentPerformingOperationValid(post.AuthorId))
            {
                throw new ForbiddenException($"You're not allowed to perform this task");
            }

            await postRepository.AssignCategoriesAsync(id, categoryIds);

            return categories.Select(cat => cat.Id).ToList();
        }

        public async Task<PostDto> CreateAsync(PostCreateRequest request)
        {
            if (request == null)
            {
                throw new ArgumentException($"{nameof(request)} cannot be null.");
            }

            ValidatePostStatus(request.Status);

            // Validates User before creating Post.
            _ = await identityApi.GetUserByIdAsync(request.AuthorId) ?? throw new NotFoundException($"Invalid User Id: {request.AuthorId}, the user with provided id could not be found!");

            var post = new Post
            {
                Title = request.Title,
                Summary = request.Summary,
                Content = request.Content,
                Slug = StringHelper.ToSlug(request.Title),
                Featured = request.Featured,
                Pinned = request.Pinned,
                CommentingEnabled = request.CommentingEnabled,
                MinutesToRead = ReadingTimeEstimatorHelper.EstimateMinutesToRead(request.Content),
                Status = request.Status,
                HashTags = request.HashTags,
                AuthorId = request.AuthorId
            };

            Post newPost = await postRepository.AddWithSaveChangesAndReturnModelAsync(post);

            ICollection<Media> media = request.Media != null && request.Media.Count > 0 ? request.Media.Adapt<ICollection<Media>>() : [];

            foreach (Media item in media)
            {
                item.InternalId = newPost.Id;
            }

            IList<MediaDto> mediaInformation = await assetApi.GetMediaInformationByIdsAsync(media.Select(m => m.AssetId!));

            await mediaRepository.AddRangeAsync(media);

            var result = newPost.Adapt<PostDto>();
            result.Media = mediaInformation;

            return result;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            Post post = await postRepository.GetEntityByIdAsync(id);

            if (IsCurrentPerformingOperationValid(post.AuthorId))
            {
                return await postRepository.DeleteAndSaveChangesAsync(post);
            }

            throw new ForbiddenException($"You're not allowed to update this {nameof(Post)}, reason: {nameof(Post)} is not belong to current user");
        }

        public async Task<PostDto> GetByIdAsync(string id)
        {
            Post post = await postRepository.GetEntityWithRelationByIdAsync(id) ?? throw new NotFoundException($"No {nameof(Post)} was found by id:{id}");

            var result = post.Adapt<PostDto>();

            if (post.Media != null && post.Media.Count > 0)
            {
                result.Media = await assetApi.GetMediaInformationByIdsAsync(post.Media.Select(m => m.AssetId!));
            }

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

            var result = posts.Adapt<IList<PostDto>>();

            AuthorDto author = await identityApi.GetUserByIdAsync(authorId) ?? throw new NotFoundException($"{nameof(author)} is not found!");

            foreach (PostDto post in result)
            {
                post.Author = author;
            }

            return result;
        }

        public async Task<PaginatedResponse<PostDto>> SearchAsync(SearchRequest request)
        {
            IQueryable<Post> predicate(IQueryable<Post> post) => post.Where(x => x.Content.Contains(request.Keyword)
                                                              || (!IsActionPerformByAdmin(LoginSession) && !x.Status.Equals(nameof(PostStatus.Craft))));

            PaginatedResponse<Post> result = await postRepository.SearchWithPaginatedResponseAsync(request.PageNumber, request.PageSize, predicate);

            return new PaginatedResponse<PostDto>(result.Data.Adapt<IReadOnlyCollection<PostDto>>(), result.TotalCount, result.PageNumber, result.TotalPages);
        }

        public async Task<PostDto> UpdateAsync(PostUpdateRequest request)
        {
            ValidatePostStatus(request.Status);
            Post post = request.Adapt<Post>();

            if (IsCurrentPerformingOperationValid(post.AuthorId))
            {
                return (await postRepository.UpdateWithSaveChangesAndReturnModelAsync(post)).Adapt<PostDto>();
            }

            throw new ForbiddenException($"You're not allowed to update this {nameof(Post)}, reason: {nameof(Post)} is not belong to current user");
        }

        public async Task<bool> UpdateSatusAsync(string id, string status)
        {
            ValidatePostStatus(status);

            Post post = await postRepository.GetEntityByIdAsync(id) ?? throw new NotFoundException($"No {nameof(Post)} was found!");

            if (!IsActionPerformByAdmin(LoginSession) && post.Status.Equals(nameof(PostStatus.InReview)))
            {
                throw new InvalidOperationException($"The {nameof(Post)} is currently in review, contact admin for approval");
            }

            if (IsCurrentPerformingOperationValid(post.Id))
            {
                post.Status = status;
                return await postRepository.UpdateAndSaveChangesAsync(post);
            }

            throw new ForbiddenException($"You're not allowed to update this {nameof(Post)}, reason: {nameof(Post)} is not belong to current user");
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