using Blog.Application.Configurations.Database;
using Blog.Application.Interfaces.Repositories;
using Blog.Domain.Entities;
using Blog.Infrastructure.Configurations;
using Blog.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Blog.Infrastructure.Repositories.Providers.Blogs
{
    public class PostRepository : DbSqlConnectionEFRepositoryBase<ApplicationDbContext, Post>, IPostRepository
    {
        public PostRepository(ISqlConnectionFactory sqlConnectionFactory,
            IHttpContextAccessor httpContextAccessor,
            ILogger<PostRepository> logger) : base(sqlConnectionFactory, httpContextAccessor, logger)
        {
        }

        public async Task<IList<Post>> GetByIdsAsync(IList<string> postIds)
        {
            return await _dbContext.Posts.Where(p => postIds.Contains(p.Id) && !p.IsDeleted).ToListAsync();
        }

        async Task<IList<Post>> IPostRepository.GetByStatusAndAuthorIdAsync(string status, string authorId)
        {
            return await _dbContext.Posts.Where(p => p.Status.Equals(status) && p.AuthorId == authorId).ToListAsync();
        }
    }
}