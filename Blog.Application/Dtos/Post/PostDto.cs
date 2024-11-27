using Blog.Application.Dtos.Base;
using Blog.Application.Dtos.Category;
using Blog.Application.Dtos.Comment;
using Blog.Application.Dtos.Interaction;

namespace Blog.Application.Dtos.Post
{
    public class PostDto : BaseDto
    {
        public string PostId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public CategoryDto? Category { get; set; }

        public IList<string> Tags { get; set; } = [];

        public string AuthorId { get; set; }

        public IList<CommentDto> Comments { get; set; } = [];

        public IList<InteractionDto> Interactions { get; set; } = [];
    }
}