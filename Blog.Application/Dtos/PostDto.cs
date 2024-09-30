using System.ComponentModel.DataAnnotations;

namespace Blog.Application.Dtos
{
    public class PostDto : BaseDto<string>
    {
        public string PostId => Id;

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public CategoryDto Category { get; set; }

        public ICollection<string?> Tags { get; set; } = [];

        [Required]
        public UserDto User { get; set; }

        public IList<CommentDto> Comments { get; set; } = [];

        public IList<InteractionDto> Interactions { get; set; } = [];
    }
}