using Blog.Application.Dtos;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;
using Blog.Domain.Common;
using Blog.Domain.Entities;
using Mapster;

namespace Blog.Application.Services
{
    public class PostService(IPostRepository postRepository) : IPostService
    {
        public async Task<PostDto> CreateAsync(PostDto post)
        {
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