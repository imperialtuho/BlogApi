namespace Blog.Domain.Entities
{
    public class Post : BaseEntity<string>
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public string? Url { get; set; }

        // Foreign Keys

        public string UserId { get; set; }

        public string? CategoryId { get; set; }

        // Navigation props
        public Category? Category { get; set; }

        public ICollection<Comment>? Comments { get; set; }

        public ICollection<PostTag>? PostTags { get; set; }

        public ICollection<Interaction>? Interactions { get; set; }
    }
}