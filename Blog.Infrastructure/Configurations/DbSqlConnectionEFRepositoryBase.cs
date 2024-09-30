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
        protected DbSqlConnectionEFRepositoryBase(ISqlConnectionFactory sqlConnectionFactory, IHttpContextAccessor httpContextAccessor) : base(CreateDbContextOptions(sqlConnectionFactory), sqlConnectionFactory, httpContextAccessor)
        {
            sqlConnectionFactory.SetConnectionStringType(ConnectionStringType.DefaultConnection);
        }

        private static DbContextOptions<C> CreateDbContextOptions(ISqlConnectionFactory sqlConnectionFactory)
        {
            (string? connectionString, ConnectionStringType dbType) = sqlConnectionFactory.GetConnectionStringAndDbType();
            var optionsBuilder = new DbContextOptionsBuilder<C>();

            if (connectionString != null)
            {
                switch (dbType)
                {
                    case ConnectionStringType.PostgresqlConnection:
                        optionsBuilder.UseNpgsql(connectionString);
                        break;

                    case ConnectionStringType.SqlServerConnection:
                        optionsBuilder.UseSqlServer(connectionString);
                        break;

                    default:
                        optionsBuilder.UseSqlServer(connectionString);
                        break;
                }
            }

            return optionsBuilder.Options;
        }
    }
}