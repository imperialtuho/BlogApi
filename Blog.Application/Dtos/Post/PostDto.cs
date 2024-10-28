using Blog.Application.Dtos.Category;

namespace Blog.Application.Dtos.Post
{
    public class PostDto : BaseDto<string>
    {
        public string PostId => Id;

        public string Title { get; set; }

        public string Content { get; set; }

        public CategoryDto? Category { get; set; }

        public ICollection<TagDto> Tags { get; set; } = [];

        public string UserId { get; set; }

        public IList<CommentDto> Comments { get; set; } = [];

        public IList<InteractionDto> Interactions { get; set; } = [];
    }
}