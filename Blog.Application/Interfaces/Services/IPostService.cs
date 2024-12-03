using Blog.Application.Dtos.Post;
using Blog.Domain.Common;

namespace Blog.Application.Interfaces.Services
{
    public interface IPostService
    {
        Task<PaginatedResponse<PostDto>> SearchAsync(SearchRequest request);

        Task<PostDto> GetByIdAsync(string id);

        Task<PostDto> CreateAsync(PostCreateRequest request);

        Task<PostDto> UpdateAsync(PostUpdateRequest request);

        Task<bool> DeleteAsync(string id);

        Task<IList<string>> AssignCategoryToPostAsync(string id, IList<string> categoryIds);
    }
}