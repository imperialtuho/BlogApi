namespace Blog.Domain.Entities
{
    public class Comment : BaseEntity<string>
    {
        public string Content { get; set; }

        // Foreign Keys
        public string UserId { get; set; }

        public User User { get; set; }

        public string PostId { get; set; }

        public Post Post { get; set; }
    }
}