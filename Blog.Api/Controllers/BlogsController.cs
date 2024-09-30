using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers
{
    /// <summary>
    /// The BlogsController.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="blogService">The blogService.</param>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BlogsController(ILogger<BlogsController> logger, IBlogService blogService) : ControllerBase
    {
        /// <summary>
        /// Gets post by id async.
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
                return Ok(await blogService.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format(ErrorLogMessage, nameof(BlogsController), nameof(GetByIdAsync), ex.Message);
                logger.LogError(ex, errorMsg);

                throw new UnhandledException(ex.Message);
            }
        }

        /// <summary>
        /// Creates post.
        /// </summary>
        /// <param name="post">The post.</param>
        /// <returns>ActionResult{PostDto}.</returns>
        /// <exception cref="UnhandledException">The UnhandledException.</exception>
        [HttpPost]
        public async Task<ActionResult<PostDto>> CreateAsync(PostDto post)
        {
            try
            {
                return Ok(await blogService.CreateAsync(post));
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format(ErrorLogMessage, nameof(BlogsController), nameof(GetByIdAsync), ex.Message);
                logger.LogError(ex, errorMsg);

                throw new UnhandledException(ex.Message);
            }
        }
    }
}