namespace Blog.Application.Dtos
{
    public class InteractionDto : BaseDto<string>
    {
        public string InteractionId => Id;

        public string PostId { get; set; }

        public string UserId { get; set; }

        public string Content { get; set; }

        public string Type { get; set; }
    }
}