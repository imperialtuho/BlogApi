using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Repositories
{
    public interface IPostRepository : IEntityFrameworkGenericRepository<Post>
    {
        Task<IList<Post>> GetByIdsAsync(IList<string> postIds);

        Task<IList<Post>> GetByStatusAndAuthorIdAsync(string status, string authorId);
    }
}