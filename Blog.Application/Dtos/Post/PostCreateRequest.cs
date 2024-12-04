using Blog.Application.Dtos.Media;

namespace Blog.Application.Dtos.Post
{
    public class PostCreateRequest
    {
        public string Title { get; set; }

        public string Summary { get; set; }

        public string Content { get; set; }

        public bool Featured { get; set; }

        public bool Pinned { get; set; }

        public string Language { get; set; }

        public bool CommentingEnabled { get; set; }

        public string Status { get; set; }

        public IList<string>? HashTags { get; set; }

        public string AuthorId { get; set; }

        public IList<MediaRequest>? Media { get; set; }
    }
}