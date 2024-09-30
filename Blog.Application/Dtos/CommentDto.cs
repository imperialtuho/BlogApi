namespace Blog.Application.Dtos
{
    public class CommentDto : BaseDto<string>
    {
        public string CommentId => Id;

        public string UserId { get; set; }

        public string PostId { get; set; }

        public string Content { get; set; }
    }
}