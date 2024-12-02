namespace Blog.Domain.Entities
{
    public class Post : BaseEntity<string>
    {
        public required string Title { get; set; }

        public string Summary { get; set; }

        public required string Content { get; set; }

        public string? Url { get; set; }

        // Foreign Keys
        public required string AuthorId { get; set; }

        public string? CategoryId { get; set; }

        // Navigation props
        public Category? Category { get; set; }

        public ICollection<Comment>? Comments { get; set; }

        public IList<string>? Tags { get; set; }

        public ICollection<Interaction>? Interactions { get; set; }
    }
}