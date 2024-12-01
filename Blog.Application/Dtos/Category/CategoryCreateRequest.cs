namespace Blog.Application.Dtos.Category
{
    public class CategoryCreateRequest
    {
        public string? Name { get; set; }

        public IList<string>? PostIds { get; set; }

        public string? UserId { get; set; }
    }
}