namespace Blog.Domain.Entities
{
    public class Category : BaseEntity<string>
    {
        public string Title { get; set; }

        public string Label { get; set; }

        public string? Description { get; set; }

        public string Slug { get; set; }

        public int DisplayPosition { get; set; }

        public string UserId { get; set; }

        // Navigation Property
        public ICollection<PostCategory>? PostCategories { get; set; }

        public ICollection<Media>? Media { get; set; }
    }
}