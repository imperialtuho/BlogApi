using Blog.Application.Configurations.Database;
using Blog.Application.Interfaces.Repositories;
using Blog.Domain.Entities;
using Blog.Domain.Enums;
using Blog.Infrastructure.Configurations;
using Blog.Infrastructure.Database;
using Microsoft.AspNetCore.Http;

namespace Blog.Infrastructure.Repositories.Providers.Blogs
{
    public class PostRepository : DbSqlConnectionEFRepositoryBase<ApplicationDbContext, Post>, IPostRepository
    {
        public PostRepository(ISqlConnectionFactory sqlConnectionFactory, IHttpContextAccessor httpContextAccessor) : base(sqlConnectionFactory, httpContextAccessor)
        {
            sqlConnectionFactory.SetConnectionStringType(ConnectionStringType.SqlServerConnection);
        }
    }
}