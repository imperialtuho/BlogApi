using Blog.Application.Dtos.Post;
using Blog.Application.Interfaces.ExternalProviders;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;
using Blog.Domain.Common;
using Blog.Domain.Entities;
using Mapster;

namespace Blog.Application.Services
{
    /// <summary>
    /// The Post Service.
    /// </summary>
    /// <param name="postRepository">The postRepository.</param>
    /// <param name="identityApi">The identityApi.</param>
    public class PostService(IPostRepository postRepository, IIdentityApi identityApi) : IPostService
    {
        public async Task<PostDto> CreateAsync(PostCreateRequest request)
        {
            if (request == null)
            {
                throw new InvalidOperationException($"{nameof(request)} cannot be null.");
            }

            // Validates User before creating Post.
            _ = await identityApi.GetUserByIdAsync(request.UserId) ?? throw new InvalidOperationException($"Invalid User Id: {request.UserId}, the user with provided id could not be found!");

            var post = new Post
            {
                TenantId = request.TenantId,
                CreatedDate = request.CreatedDate,
                CreatedBy = request.CreatedBy,
                ModifiedDate = request.ModifiedDate,
                ModifiedBy = request.ModifiedBy,
                IsActive = request.IsActive,
                Title = request.Title,
                Content = request.Content,
                Url = request.Url,
                CategoryId = request.CategoryId,
                UserId = request.UserId,
                PostTags = request.TagIds.Select(tagId => new PostTag { TagId = tagId }).ToList()
            };

            Post newPost = await postRepository.AddWithSaveChangesAndReturnModelAsync(post);

            return newPost.Adapt<PostDto>();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            Post post = await postRepository.GetEntityByIdAsync(id);

            return await Task.Run(() => postRepository.DeleteAndSaveChangesAsync(post).IsCompleted);
        }

        public async Task<PaginatedResponse<PostDto>> SearchWithPaginatedResponseAsync(int pageNumber = 1, int pageSize = 10, Func<IQueryable<Post>, IQueryable<Post>>? predicate = null)
        {
            return (await postRepository.SearchWithPaginatedResponseAsync(pageNumber, pageSize, predicate)).Adapt<PaginatedResponse<PostDto>>();
        }

        public async Task<PostDto> GetByIdAsync(string id)
        {
            Post post = await postRepository.GetEntityWithRelationByIdAsync(id);

            return post.Adapt<PostDto>();
        }

        public Task<PostDto> UpdateAsync(PostUpdateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}