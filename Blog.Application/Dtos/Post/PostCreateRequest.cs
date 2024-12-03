namespace Blog.Application.Dtos.Post
{
    public class PostCreateRequest
    {
        public bool IsPublished { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public string? Url { get; set; }
        public string? CategoryId { get; set; } // ID of the existing category
        public required string AuthorId { get; set; } // ID of the existing author
        public List<string>? Tags { get; set; }
    }
}