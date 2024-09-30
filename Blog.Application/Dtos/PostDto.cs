using System.ComponentModel.DataAnnotations;

namespace Blog.Application.Dtos
{
    public class PostDto : BaseDto<string>
    {
        public string PostId => Id;

        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Content { get; set; }

        public CategoryDto? Category { get; set; }

        public ICollection<TagDto> Tags { get; set; } = [];

        [Required]
        public required UserDto User { get; set; }

        public IList<CommentDto> Comments { get; set; } = [];

        public IList<InteractionDto> Interactions { get; set; } = [];
    }
}