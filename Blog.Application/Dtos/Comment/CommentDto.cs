using Blog.Application.Dtos.Base;

namespace Blog.Application.Dtos.Comment
{
    public class CommentDto : BaseDto
    {
        public string Id { get; set; }

        public string Content { get; set; }

        // Navigation properties
        public string UserId { get; set; }

        public string PostId { get; set; }
    }
}