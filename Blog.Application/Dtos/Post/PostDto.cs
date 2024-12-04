using Blog.Application.Dtos.Author;
using Blog.Application.Dtos.Base;
using Blog.Application.Dtos.Comment;
using Blog.Application.Dtos.Interaction;
using Blog.Application.Dtos.Media;
using Blog.Application.Dtos.Tag;

namespace Blog.Application.Dtos.Post
{
    public class PostDto : BaseDto
    {
        public string Id { set; get; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string Content { get; set; }

        public string Slug { get; set; }

        public bool Featured { get; set; }

        public bool Pinned { get; set; }

        public string Language { get; set; }

        public bool CommentingEnabled { get; set; }

        public int MinutesToRead { get; set; }

        public string Status { get; set; }

        public IList<string>? HashTags { get; set; }

        public IList<MediaDto>? Media { get; set; }

        public IList<string>? CategoryIds { get; set; }

        public AuthorDto Author { get; set; }

        public IList<TagDto> Tags { get; set; } = [];

        public IList<CommentDto> Comments { get; set; } = [];

        public IList<InteractionDto> Interactions { get; set; } = [];
    }
}