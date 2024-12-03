namespace Blog.Domain.Entities
{
    public class PostCategory
    {
        public string CategoryId { get; set; }

        public Category Category { get; set; }

        public string PostId { get; set; }

        public Post Post { get; set; }
    }
}