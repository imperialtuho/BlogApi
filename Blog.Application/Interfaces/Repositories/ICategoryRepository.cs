using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IEntityFrameworkGenericRepository<Category>
    {
    }
}