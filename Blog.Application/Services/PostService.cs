using Blog.Application.Dtos.Media;
using Blog.Application.Dtos.Post;
using Blog.Application.Interfaces.ExternalProviders;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;
using Blog.Domain.Common;
using Blog.Domain.Entities;
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
        public async Task<IList<string>> AssignCategoryToPostAsync(string id, IList<string> categoryIds)
        {
            IList<Category> categories = await categoryRepository.GetByIdsAsync(categoryIds);

            if (categories == null || categories.Count == 0)
            {
                throw new NotFoundException($"There was no {nameof(Category)} found!");
            }

            await postRepository.AssignCategoriesAsync(id, categoryIds);

            return categories.Select(cat => cat.Id).ToList();
        }

        public async Task<PostDto> CreateAsync(PostCreateRequest request)
        {
            if (request == null)
            {
                throw new InvalidOperationException($"{nameof(request)} cannot be null.");
            }

            // Validates User before creating Post.
            _ = await identityApi.GetUserByIdAsync(request.AuthorId) ?? throw new InvalidOperationException($"Invalid User Id: {request.AuthorId}, the user with provided id could not be found!");

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

            if (IsActionPerformByAdmin(LoginSession) || post.AuthorId.Equals(LoginSession?.UserId.ToString()))
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

        public async Task<PaginatedResponse<PostDto>> SearchAsync(SearchRequest request)
        {
            IQueryable<Post> predicate(IQueryable<Post> post) => post.Where(x => x.Content.Contains(request.Keyword));
            PaginatedResponse<Post> result = await postRepository.SearchWithPaginatedResponseAsync(request.PageNumber, request.PageSize, predicate);

            return new PaginatedResponse<PostDto>(result.Data.Adapt<IReadOnlyCollection<PostDto>>(), result.TotalCount, result.PageNumber, result.TotalPages);
        }

        public async Task<PostDto> UpdateAsync(PostUpdateRequest request)
        {
            Post post = request.Adapt<Post>();

            if (IsActionPerformByAdmin(LoginSession) || post.AuthorId.Equals(LoginSession?.UserId.ToString()))
            {
                return (await postRepository.UpdateWithSaveChangesAndReturnModelAsync(post)).Adapt<PostDto>();
            }

            throw new ForbiddenException($"You're not allowed to update this {nameof(Post)}, reason: {nameof(Post)} is not belong to current user");
        }
    }
}