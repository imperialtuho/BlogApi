using Blog.Application.Configurations.Database;
using Blog.Domain.Entities;
using Blog.Domain.Enums;
using Blog.Infrastructure.Repositories.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure.Configurations
{
    public abstract class DbSqlConnectionEFRepositoryBase<C, T> : EntityFrameworkGenericRepository<C, T>
        where T : BaseEntity<string>
        where C : DbContext, new()
    {
        protected DbSqlConnectionEFRepositoryBase(ISqlConnectionFactory sqlConnectionFactory, IHttpContextAccessor httpContextAccessor)
            : base(CreateDbContextOptions(sqlConnectionFactory, ConnectionStringType.SqlServerConnection), sqlConnectionFactory, httpContextAccessor)
        {
        }
    }
}