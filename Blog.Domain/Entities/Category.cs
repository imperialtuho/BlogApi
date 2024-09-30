namespace Blog.Domain.Entities
{
    public class Category : BaseEntity<string>
    {
        public string Name { get; set; }

        public ICollection<Post> Posts { get; set; } = [];
    }
}