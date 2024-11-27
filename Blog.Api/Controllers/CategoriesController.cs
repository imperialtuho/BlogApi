using Blog.Application.Dtos.Category;

namespace Blog.Api.Controllers
{
    /// <summary>
    /// The CategoriesController.
    /// </summary>
    /// <param name="categoryService">The categoryService.</param>
    public class CategoriesController(ICategoryService categoryService) : BaseController
    {
        /// <summary>
        /// Gets Category by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>ActionResult{CategoryDto}.</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByIdAsync([FromRoute] string id)
        {
            return ReturnResult(await categoryService.GetByIdAsync(id), HttpStatusCode.OK);
        }

        /// <summary>
        /// Creates Category.
        /// </summary>
        /// <param name="Category">The Category.</param>
        /// <returns>ActionResult{CategoryDto}.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CategoryCreateRequest Category)
        {
            return ReturnResult(await categoryService.CreateAsync(Category), HttpStatusCode.Created);
        }

        /// <summary>
        /// Updates Category.
        /// </summary>
        /// <param name="id">The request id.</param>
        /// <param name="request">The request.</param>
        /// <returns>The updated category on successfully update action.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromBody] CategoryUpdateRequest request)
        {
            if (!id.Equals(request.Id, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest($"Invalid param \'id\' between route and payload model!: From route: {id}, from payload: {request.Id}");
            }

            return ReturnResult(await categoryService.UpdateAsync(request), HttpStatusCode.OK);
        }

        /// <summary>
        /// Deletes Category by id.
        /// </summary>
        /// <param name="id">The request id.</param>
        /// <returns>True/False based on action result.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id)
        {
            return ReturnResult(await categoryService.DeleteAsync(id), HttpStatusCode.OK);
        }
    }
}