using Blog.Application.Configurations.Database;
using Blog.Application.Interfaces.Repositories;
using Blog.Domain.Entities;
using Blog.Infrastructure.Configurations;
using Blog.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Blog.Infrastructure.Repositories.Providers.Blogs
{
    public class CategoryRepository : DbSqlConnectionEFRepositoryBase<ApplicationDbContext, Category>, ICategoryRepository
    {
        public CategoryRepository(ISqlConnectionFactory sqlConnectionFactory, IHttpContextAccessor httpContextAccessor) : base(sqlConnectionFactory, httpContextAccessor)
        {
        }

        public async Task<Category?> GetByNameAsync(string name, string? userId = null, bool isGlobal = false)
        {
            Expression<Func<Category, bool>> predicate = category =>
                category.Name.Equals(name) &&
                (userId == null || userId.Equals(category.UserId)) && (string.IsNullOrEmpty(category.UserId) || category.IsGlobal);

            return await _dbContext.Categories.FirstOrDefaultAsync(predicate);
        }
    }
}