using Blog.Application.Dtos;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;
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