namespace Blog.Domain.Entities
{
    public class Category : BaseEntity<string>
    {
        public string Title { get; set; }

        public string Label { get; set; }

        public string? Description { get; set; }

        public string Slug { get; set; }

        public int DisplayPosition { get; set; }

        public string AuthorId { get; set; }

        public string? CoverImageUrl { get; set; }

        // Navigation Property
        public ICollection<PostCategory>? PostCategories { get; set; }
    }
}