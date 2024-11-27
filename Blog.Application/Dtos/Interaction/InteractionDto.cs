using Blog.Application.Dtos.Base;

namespace Blog.Application.Dtos.Interaction
{
    public class InteractionDto : BaseDto
    {
        public string InteractionId { get; set; }

        public string PostId { get; set; }

        public string UserId { get; set; }

        public string Content { get; set; }

        public string Type { get; set; }
    }
}