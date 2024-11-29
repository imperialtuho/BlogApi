namespace Blog.Application.Dtos.Category
{
    public class CategoryUpdateRequest
    {
        public required string Id { get; set; }

        public string? Name { get; set; }

        public IList<string>? PostIds { get; set; } = [];

        public bool IsActive { get; set; }
    }
}