using Blog.Application.Dtos.Post;
using Blog.Application.Interfaces.ExternalProviders;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;
using Blog.Domain.Common;
using Blog.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace Blog.Application.Services
{
    /// <summary>
    /// The Post Service.
    /// </summary>
    /// <param name="postRepository">The postRepository.</param>
    /// <param name="identityApi">The identityApi.</param>
    public class PostService(IPostRepository postRepository, IIdentityApi identityApi, IHttpContextAccessor httpContextAccessor) : BaseService(httpContextAccessor), IPostService
    {
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
                IsActive = request.IsActive,
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
                return await Task.Run(() => postRepository.DeleteAndSaveChangesAsync(post).IsCompleted);
            }

            throw new InvalidOperationException("You're not allowed to delete this post, reason: Post is not belong to current user");
        }

        public async Task<PostDto> GetByIdAsync(string id)
        {
            Post post = await postRepository.GetEntityWithRelationByIdAsync(id);

            return post.Adapt<PostDto>();
        }

        public async Task<PaginatedResponse<PostDto>> SearchWithPaginatedResponseAsync(int pageNumber = 1, int pageSize = 10, Func<IQueryable<Post>, IQueryable<Post>>? predicate = null)
        {
            return (await postRepository.SearchWithPaginatedResponseAsync(pageNumber, pageSize, predicate)).Adapt<PaginatedResponse<PostDto>>();
        }

        public Task<PostDto> UpdateAsync(PostUpdateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}