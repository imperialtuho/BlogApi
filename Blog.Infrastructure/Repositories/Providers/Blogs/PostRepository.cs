using Blog.Application.Configurations.Database;
using Blog.Application.Dtos;
using Blog.Application.Interfaces.Repositories;
using Blog.Domain.Entities;
using Blog.Domain.Enums;
using Blog.Domain.Exceptions;
using Blog.Infrastructure.Configurations;
using Blog.Infrastructure.Database;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace Blog.Infrastructure.Repositories.Providers.Blogs
{
    public class PostRepository : DbSqlConnectionEFRepositoryBase<ApplicationDbContext, Post>, IPostRepository
    {
        public PostRepository(ISqlConnectionFactory sqlConnectionFactory, IHttpContextAccessor httpContextAccessor) : base(sqlConnectionFactory, httpContextAccessor)
        {
            sqlConnectionFactory.SetConnectionStringType(ConnectionStringType.SqlServerConnection);
        }

        public async Task<Post> CreateAsync(PostDto post)
        {
            if (post == null)
            {
                throw new InvalidOperationException($"{nameof(post)} cannot be null.");
            }

            var postToAdd = post.Adapt<Post>();

            await AddAndSaveChangesAsync(postToAdd);

            return postToAdd;
        }

        public async Task DeleteAsync(string id)
        {
            Post? postToRemove = await GetByIdAsync(id) ?? throw new NotFoundException($"{nameof(Post)} with provided id: {id} is not found.");
            await DeleteAndSaveChangesAsync(postToRemove);
        }

        public async Task<Post> GetByIdAsync(string id)
        {
            return await GetEntityByIdAsync(id);
        }

        public Task<Post> UpdateAsync(PostDto post)
        {
            throw new NotImplementedException();
        }
    }
}