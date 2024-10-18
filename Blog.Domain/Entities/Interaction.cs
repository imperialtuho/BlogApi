namespace Blog.Domain.Entities
{
    public class Interaction : BaseEntity<string>
    {
        public string Content { get; set; }

        public string Type { get; set; }

        public string UserId { get; set; }

        public string PostId { get; set; }

        public Post Post { get; set; }
    }
}