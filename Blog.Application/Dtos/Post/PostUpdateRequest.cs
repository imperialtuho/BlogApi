namespace Blog.Application.Dtos.Post
{
    public class PostUpdateRequest
    {
        public required string Id { get; set; } // ID of the post to be updated
        public required string Title { get; set; }
        public required string Content { get; set; }
        public bool IsActive { get; set; }
        public string? Url { get; set; }
        public string? CategoryId { get; set; } // ID of the existing category
        public required string AuthorId { get; set; } // ID of the existing user
        public List<string>? Tags { get; set; } // IDs of the existing tags
    }
}