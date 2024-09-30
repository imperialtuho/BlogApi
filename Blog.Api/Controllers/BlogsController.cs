using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers
{
    /// <summary>
    /// The BlogsController.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="postService">The blogService.</param>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BlogsController(ILogger<BlogsController> logger, IPostService postService) : ControllerBase
    {
        /// <summary>
        /// Gets Post by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult{PostDto}.</returns>
        /// <exception cref="UnhandledException">The UnhandledException.</exception>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<PostDto>> GetByIdAsync(string id)
        {
            try
            {
                return Ok(await postService.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format(ErrorLogMessage, nameof(BlogsController), nameof(GetByIdAsync), ex.Message);
                logger.LogError(ex, errorMsg);

                throw new UnhandledException(ex.Message);
            }
        }

        /// <summary>
        /// Creates Post.
        /// </summary>
        /// <param name="post">The post.</param>
        /// <returns>ActionResult{PostDto}.</returns>
        /// <exception cref="UnhandledException">The UnhandledException.</exception>
        [HttpPost]
        public async Task<ActionResult<PostDto>> CreateAsync(PostDto post)
        {
            try
            {
                return Ok(await postService.CreateAsync(post));
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format(ErrorLogMessage, nameof(BlogsController), nameof(CreateAsync), ex.Message);
                logger.LogError(ex, errorMsg);

                throw new UnhandledException(ex.Message);
            }
        }

        /// <summary>
        /// Updates Post.
        /// </summary>
        /// <param name="id">The request id.</param>
        /// <param name="post">The post.</param>
        /// <returns>The updated post on successfully update action.</returns>
        /// <exception cref="UnhandledException">The UnhandledException.</exception>
        [HttpPut("{id}")]
        public async Task<ActionResult<PostDto>> UpdateAsync(string id, PostDto post)
        {
            try
            {
                if (!id.Equals(post.Id, StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest($"Invalid param \'id\' between route and payload model!: From route: {id}, from payload: {post.Id}");
                }

                return (Ok(await postService.UpdateAsync(post)));
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format(ErrorLogMessage, nameof(BlogsController), nameof(UpdateAsync), ex.Message);
                logger.LogError(ex, errorMsg);

                throw new UnhandledException(ex.Message);
            }
        }

        /// <summary>
        /// Deletes Post by id.
        /// </summary>
        /// <param name="id">The request id.</param>
        /// <returns>True/False based on action result.</returns>
        /// <exception cref="UnhandledException">The UnhandledException.</exception>
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteAsync(string id)
        {
            try
            {
                return await postService.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format(ErrorLogMessage, nameof(BlogsController), nameof(DeleteAsync), ex.Message);
                logger.LogError(ex, errorMsg);

                throw new UnhandledException(ex.Message);
            }
        }
    }
}