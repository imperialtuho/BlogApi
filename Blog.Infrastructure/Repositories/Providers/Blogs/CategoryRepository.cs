using Blog.Application.Configurations.Database;
using Blog.Application.Interfaces.Repositories;
using Blog.Domain.Entities;
using Blog.Infrastructure.Configurations;
using Blog.Infrastructure.Database;
using Microsoft.AspNetCore.Http;

namespace Blog.Infrastructure.Repositories.Providers.Blogs
{
    public class CategoryRepository : DbSqlConnectionEFRepositoryBase<ApplicationDbContext, Category>, ICategoryRepository
    {
        public CategoryRepository(ISqlConnectionFactory sqlConnectionFactory, IHttpContextAccessor httpContextAccessor) : base(sqlConnectionFactory, httpContextAccessor)
        {
        }
    }
}