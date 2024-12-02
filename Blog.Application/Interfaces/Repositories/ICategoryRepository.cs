using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IEntityFrameworkGenericRepository<Category>
    {
        Task<Category?> GetByNameAsync(string name, string? userId = null, bool isGlobal = false);
    }
}