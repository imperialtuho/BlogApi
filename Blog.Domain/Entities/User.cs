namespace Blog.Domain.Entities
{
    public class User : BaseEntity<string>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<Post> Posts { get; set; } = [];

        public ICollection<Comment> Comments { get; set; } = [];

        public ICollection<Interaction> Interactions { get; set; } = [];
    }
}