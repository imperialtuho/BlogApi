using Blog.Application.Dtos.Post;
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
    /// <summary>
    /// The Post Service.
    /// </summary>
    /// <param name="postRepository">The postRepository.</param>
    /// <param name="identityApi">The identityApi.</param>
    public class PostService(IPostRepository postRepository, ICategoryRepository categoryRepository, IIdentityApi identityApi, IHttpContextAccessor httpContextAccessor) : BaseService(httpContextAccessor), IPostService
    {
        public async Task<PostDto> CreateAsync(PostCreateRequest request)
        {
            if (request == null)
            {
                throw new InvalidOperationException($"{nameof(request)} cannot be null.");
            }

            if (!string.IsNullOrEmpty(request.CategoryId))
            {
                // Validate category
                _ = await categoryRepository.GetEntityByIdAsync(request.CategoryId) ?? throw new InvalidOperationException($"Invalid {nameof(Category)}: {request.CategoryId}, the {nameof(Category)} with the provided id could not be found!");
            }

            // Validates User before creating Post.
            _ = await identityApi.GetUserByIdAsync(request.AuthorId) ?? throw new InvalidOperationException($"Invalid User Id: {request.AuthorId}, the user with the provided id could not be found!");

            var post = new Post
            {
                Title = request.Title,
                Content = request.Content,
                Url = request.Url,
                CategoryId = request.CategoryId,
                AuthorId = request.AuthorId,
                Tags = request.Tags
            };

            Post newPost = await postRepository.AddWithSaveChangesAndReturnModelAsync(post);

            return newPost.Adapt<PostDto>();
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
            Post post = await postRepository.GetEntityWithRelationByIdAsync(id);

            return post.Adapt<PostDto>();
        }

        public async Task<PaginatedResponse<PostDto>> SearchAsync(SearchRequest request)
        {
            Func<IQueryable<Post>, IQueryable<Post>> predicate = posts =>
            {
                // Base filter: match keyword in Name
                IQueryable<Post> query = posts.Where(entity => entity.Content.Contains(request.Keyword));

                // Filter out deleted posts if not including deleted
                if (!request.IsIncludingDelete)
                {
                    query = query.Where(entity => !entity.IsDeleted);
                }

                // Further filter by active status if not including active-only
                if (!request.IsIncludingActiveOnly)
                {
                    query = query.Where(entity => entity.IsActive);
                }

                return query;
            };
            PaginatedResponse<Post> result = await postRepository.SearchWithPaginatedResponseAsync(request.PageNumber, request.PageSize, predicate);

            return new PaginatedResponse<PostDto>(result.Items.Adapt<IReadOnlyCollection<PostDto>>(), result.TotalCount, result.PageNumber, result.TotalPages);
        }

        public async Task<PostDto> UpdateAsync(PostUpdateRequest request)
        {
            if (!string.IsNullOrEmpty(request.CategoryId))
            {
                // Validate category
                _ = await categoryRepository.GetEntityByIdAsync(request.CategoryId) ?? throw new InvalidOperationException($"Invalid {nameof(Category)}: {request.CategoryId}, the {nameof(Category)} with the provided id could not be found!");
            }

            Post post = request.Adapt<Post>();

            if (IsActionPerformByAdmin(LoginSession) || post.AuthorId.Equals(LoginSession?.UserId.ToString()))
            {
                return (await postRepository.UpdateWithSaveChangesAndReturnModelAsync(post)).Adapt<PostDto>();
            }

            throw new ForbiddenException($"You're not allowed to update this {nameof(Post)}, reason: {nameof(Post)} is not belong to current user");
        }
    }
}