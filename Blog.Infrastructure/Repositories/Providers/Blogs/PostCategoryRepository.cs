using Blog.Application.Configurations.Database;
using Blog.Application.Interfaces.Repositories;
using Blog.Domain.Entities;
using Blog.Infrastructure.Configurations;
using Blog.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Blog.Infrastructure.Repositories.Providers.Blogs
{
    public class PostCategoryRepository : DbSqlConnectionEFRepositoryBase<ApplicationDbContext, PostCategory>, IPostCategoryRepository
    {
        public PostCategoryRepository(ISqlConnectionFactory sqlConnectionFactory, IHttpContextAccessor httpContextAccessor, ILogger<PostCategoryRepository> logger) : base(sqlConnectionFactory, httpContextAccessor, logger)
        {
        }

        public async Task<bool> AssignCategoriesAsync(string id, IList<string> categoryIds)
        {
            List<PostCategory>? postCategories = categoryIds.Select(categoryId => new PostCategory
            {
                CategoryId = categoryId,
                PostId = id
            }).ToList();

            return await AddRangeAndSaveChangesAsync(postCategories);
        }

        public async Task<bool> UnAssignCategoriesAsync(string id, IList<string> categoryIds)
        {
            return await ForceDeleteWhereAsync(x => x.PostId.Equals(id) && categoryIds.Contains(x.CategoryId));
        }
    }
}