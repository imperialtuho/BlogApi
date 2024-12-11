using Blog.Application.Configurations.Database;
using Blog.Application.Interfaces.Repositories;
using Blog.Domain.Entities;
using Blog.Infrastructure.Configurations;
using Blog.Infrastructure.Database;
using Microsoft.AspNetCore.Http;

namespace Blog.Infrastructure.Repositories.Providers.Blogs
{
    public class MediaRepository : DbSqlConnectionEFRepositoryBase<ApplicationDbContext, Media>, IMediaRepository
    {
        public MediaRepository(ISqlConnectionFactory sqlConnectionFactory, IHttpContextAccessor httpContextAccessor) : base(sqlConnectionFactory, httpContextAccessor)
        {
        }
    }
}