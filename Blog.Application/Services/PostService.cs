using Blog.Application.Dtos;
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
        public async Task<PostDto> CreateAsync(PostDto post)
        {
            if (post == null)
            {
                throw new InvalidOperationException($"{nameof(post)} cannot be null.");
            }

            // Validates User before creating Post.
            _ = await identityApi.GetUserByIdAsync(post.User.UserId) ?? throw new InvalidOperationException($"Invalid User Id: {post.User.UserId}, the user with provided id could not be found!");

            return (await postRepository.CreateAsync(post)).Adapt<PostDto>();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await Task.Run(() => postRepository.DeleteAsync(id).IsCompleted);
        }

        public async Task<PaginatedResponse<PostDto>> SearchWithPaginatedResponseAsync(int pageNumber = 1, int pageSize = 10, Func<IQueryable<Post>, IQueryable<Post>>? predicate = null)
        {
            return (await postRepository.SearchWithPaginatedResponseAsync(pageNumber, pageSize, predicate)).Adapt<PaginatedResponse<PostDto>>();
        }

        public async Task<PostDto> GetByIdAsync(string id)
        {
            return (await postRepository.GetByIdAsync(id)).Adapt<PostDto>();
        }

        public Task<PostDto> UpdateAsync(PostDto post)
        {
            throw new NotImplementedException();
        }
    }
}