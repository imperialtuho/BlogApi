namespace Blog.Domain.Entities
{
    public class Tag : BaseEntity<string>
    {
        public string Label { get; set; }

        public string Slug { get; set; }

        // Relationships
        public ICollection<PostTag>? PostTags { get; set; }
    }
}