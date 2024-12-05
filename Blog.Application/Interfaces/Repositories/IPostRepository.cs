using Blog.Application.Dtos.Post;
using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Repositories
{
    public interface IPostRepository : IEntityFrameworkGenericRepository<Post>
    {
        Task<IList<Post>> GetByIdsAsync(IList<string> postIds);

        Task AssignCategoriesAsync(string postId, IList<string> categoryIds);

        Task<IList<Post>> GetByStatusAndAuthorIdAsync(string status, string authorId);
    }
}