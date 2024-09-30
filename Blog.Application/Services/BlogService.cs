using Blog.Application.Dtos;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;
using Mapster;

namespace Blog.Application.Services
{
    public class BlogService(IBlogRepository blogRepository) : IBlogService
    {
        public async Task<PostDto> CreateAsync(PostDto post)
        {
            return (await blogRepository.CreateAsync(post)).Adapt<PostDto>();
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<PostDto> GetByIdAsync(string id)
        {
            return (await blogRepository.GetByIdAsync(id)).Adapt<PostDto>();
        }

        public Task<PostDto> UpdateAsync(PostDto post)
        {
            throw new NotImplementedException();
        }
    }
}