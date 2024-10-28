using Blog.Application.Dtos.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers
{
    /// <summary>
    /// The CategoriesController.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="categoryService">The categoryService.</param>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController(ILogger<CategoriesController> logger, ICategoryService categoryService) : ControllerBase
    {
        /// <summary>
        /// Gets Category by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult{CategoryDto}.</returns>
        /// <exception cref="UnhandledException">The UnhandledException.</exception>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryDto>> GetByIdAsync(string id)
        {
            try
            {
                return Ok(await categoryService.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format(ErrorLogMessage, nameof(CategoriesController), nameof(GetByIdAsync), ex.Message);
                logger.LogError(ex, errorMsg);

                throw new UnhandledException(ex.Message);
            }
        }

        /// <summary>
        /// Creates Category.
        /// </summary>
        /// <param name="Category">The Category.</param>
        /// <returns>ActionResult{CategoryDto}.</returns>
        /// <exception cref="UnhandledException">The UnhandledException.</exception>
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateAsync(CategoryCreateRequest Category)
        {
            try
            {
                return Ok(await categoryService.CreateAsync(Category));
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format(ErrorLogMessage, nameof(CategoriesController), nameof(CreateAsync), ex.Message);
                logger.LogError(ex, errorMsg);

                throw new UnhandledException(ex.Message);
            }
        }

        /// <summary>
        /// Updates Category.
        /// </summary>
        /// <param name="id">The request id.</param>
        /// <param name="Category">The Category.</param>
        /// <returns>The updated category on successfully update action.</returns>
        /// <exception cref="UnhandledException">The UnhandledException.</exception>
        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryDto>> UpdateAsync(string id, CategoryUpdateRequest request)
        {
            try
            {
                if (!id.Equals(request.Id, StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest($"Invalid param \'id\' between route and payload model!: From route: {id}, from payload: {request.Id}");
                }

                return (Ok(await categoryService.UpdateAsync(request)));
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format(ErrorLogMessage, nameof(CategoriesController), nameof(UpdateAsync), ex.Message);
                logger.LogError(ex, errorMsg);

                throw new UnhandledException(ex.Message);
            }
        }

        /// <summary>
        /// Deletes Category by id.
        /// </summary>
        /// <param name="id">The request id.</param>
        /// <returns>True/False based on action result.</returns>
        /// <exception cref="UnhandledException">The UnhandledException.</exception>
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteAsync(string id)
        {
            try
            {
                return await categoryService.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format(ErrorLogMessage, nameof(CategoriesController), nameof(DeleteAsync), ex.Message);
                logger.LogError(ex, errorMsg);

                throw new UnhandledException(ex.Message);
            }
        }
    }
}