namespace Blog.Application.Dtos.Category
{
    public class CategoryUpdateRequest : BaseDto<string>
    {
        public string? Name { get; set; }

        public IList<string?> PostIds { get; set; } = [];
    }
}