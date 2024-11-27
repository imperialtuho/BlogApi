using Blog.Application.Dtos.Base;

namespace Blog.Application.Dtos.Comment
{
    public class CommentDto : BaseDto
    {
        public string CommentId { get; set; }

        public string UserId { get; set; }

        public string PostId { get; set; }

        public string Content { get; set; }
    }
}