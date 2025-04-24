using Asp.Versioning;
using Blog.Application.Dtos.Post;
using Blog.Domain.Common;

namespace Blog.Api.Controllers
{
    /// <summary>
    /// The PostsController.
    /// </summary>
    /// <param name="postService">The blogService.</param>
    [ApiVersion("1.0")]
    [Route("api/v{version:ApiVersion}/[controller]")]
    public class PostsController(IPostService postService) : BaseController
    {
        /// <summary>
        /// Gets Post by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A post after successfully getting post by id action.</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByIdAsync([FromRoute] string id)
        {
            return Result(await postService.GetByIdAsync(id), HttpStatusCode.OK);
        }

        /// <summary>
        /// Creates Post.
        /// </summary>
        /// <param name="post">The post.</param>
        /// <returns>A post after successfully performing create action.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] PostCreateRequest post)
        {
            return Result(await postService.CreateAsync(post), HttpStatusCode.Created);
        }

        /// <summary>
        /// Updates Post.
        /// </summary>
        /// <param name="id">The request id.</param>
        /// <param name="post">The post.</param>
        /// <returns>The updated post on successfully update action.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromBody] PostUpdateRequest post)
        {
            if (!id.Equals(post.Id, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest($"Invalid param \'id\' between route and payload model!: From route: {id}, from payload: {post.Id}");
            }

            return Result(await postService.UpdateAsync(post), HttpStatusCode.OK);
        }

        /// <summary>
        /// Deletes Post by id.
        /// </summary>
        /// <param name="id">The request id.</param>
        /// <returns>True/False based on action result.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id)
        {
            return Result(await postService.DeleteAsync(id), HttpStatusCode.OK);
        }

        /// <summary>
        /// Searches by keyword.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Results which matched/similar with the request's keyword.</returns>
        [HttpPost("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchAsync([FromBody] SearchRequest request)
        {
            return Result(await postService.SearchAsync(request), HttpStatusCode.OK);
        }

        /// <summary>
        /// Gets all posts by status belong to author id.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="authorId">The authorId.</param>
        /// <returns>A list of posts with the provided status blongs to author.</returns>
        [HttpGet]
        public async Task<IActionResult> GetByStatusAndUserIdAsync([FromQuery] string status, [FromQuery] string authorId)
        {
            return Result(await postService.GetByStatusAndAuthorIdAsync(status, authorId));
        }

        /// <summary>
        /// Updates post's status.
        /// </summary>
        /// <param name="id">The post id.</param>
        /// <param name="status">The status.</param>
        /// <returns>True/False based on update action.</returns>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatusAsync([FromRoute] string id, [FromBody] string status)
        {
            return Result(await postService.UpdateSatusAsync(id, status));
        }

        /// <summary>
        /// Assignes categories to a post.
        /// </summary>
        /// <param name="id">The post id.</param>
        /// <param name="categoryIds">The category ids.</param>
        /// <returns>True/False based on assign action.</returns>
        [HttpPut("{id}/assign-categories")]
        public async Task<bool> AssignCategoriesAsync(string id, [FromBody] IList<string> categoryIds)
        {
            return await postService.AssignCategoriesAsync(id, categoryIds);
        }

        /// <summary>
        /// Unassignes categories from a post.
        /// </summary>
        /// <param name="id">The post id.</param>
        /// <param name="categoryIds">The category ids.</param>
        /// <returns>True/False based on unassign action.</returns>
        [HttpPut("{id}/unassign-categories")]
        public async Task<bool> UnAssignCategoriesAsync(string id, [FromBody] IList<string> categoryIds)
        {
            return await postService.UnAssignCategoriesAsync(id, categoryIds);
        }
    }
}