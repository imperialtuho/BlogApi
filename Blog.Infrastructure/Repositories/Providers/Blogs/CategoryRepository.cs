using Blog.Application.Configurations.Database;
using Blog.Application.Interfaces.Repositories;
using Blog.Domain.Entities;
using Blog.Infrastructure.Configurations;
using Blog.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Blog.Infrastructure.Repositories.Providers.Blogs
{
    public class CategoryRepository : DbSqlConnectionEFRepositoryBase<ApplicationDbContext, Category>, ICategoryRepository
    {
        public CategoryRepository(ISqlConnectionFactory sqlConnectionFactory, IHttpContextAccessor httpContextAccessor, ILogger<CategoryRepository> logger) : base(sqlConnectionFactory, httpContextAccessor, logger)
        {
        }

        public async Task<IList<Category>> GetByIdsAsync(IList<string> ids)
        {
            return await _dbContext.Categories.Where(category => ids.Contains(category.Id)).ToListAsync();
        }

        public async Task<Category?> GetByNameAsync(string name, string? userId = null, bool isGlobal = false)
        {
            Expression<Func<Category, bool>> predicate = category => (category.Title.Equals(name) || category.Label.Equals(name)) && (userId == null || userId.Equals(category.AuthorId));

            return await _dbContext.Categories.FirstOrDefaultAsync(predicate);
        }
    }
}