namespace Blog.Domain.Entities
{
    public class Tag : BaseEntity<string>
    {
        public string Name { get; set; }

        public ICollection<PostTag> PostTags { get; set; }
    }
}