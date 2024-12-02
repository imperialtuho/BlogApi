using Blog.Application.Configurations.Database;
using Blog.Application.Interfaces.Repositories;
using Blog.Domain.Entities;
using Blog.Infrastructure.Configurations;
using Blog.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure.Repositories.Providers.Blogs
{
    public class PostRepository : DbSqlConnectionEFRepositoryBase<ApplicationDbContext, Post>, IPostRepository
    {
        public PostRepository(ISqlConnectionFactory sqlConnectionFactory, IHttpContextAccessor httpContextAccessor) : base(sqlConnectionFactory, httpContextAccessor)
        {
        }

        public async Task<IList<Post>> GetByIdsAsync(IList<string> postIds)
        {
            return await _dbContext.Posts.Where(p => postIds.Contains(p.Id) && p.IsActive && !p.IsDeleted).ToListAsync();
        }
    }
}