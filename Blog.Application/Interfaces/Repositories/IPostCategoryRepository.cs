namespace Blog.Application.Interfaces.Repositories
{
    public interface IPostCategoryRepository
    {
        Task<bool> AssignCategoriesAsync(string id, IList<string> categoryIds);

        Task<bool> UnAssignCategoriesAsync(string id, IList<string> categoryIds);
    }
}