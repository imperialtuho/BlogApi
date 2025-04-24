namespace Blog.Application.Dtos.Post
{
    public class PostCreateRequest
    {
        public string Title { get; set; }

        public string Summary { get; set; }

        public string Content { get; set; }

        public string CoverImageUrl { get; set; }

        public bool Featured { get; set; }

        public bool Pinned { get; set; }

        public bool CommentingEnabled { get; set; }

        public string Status { get; set; }

        public IList<string>? HashTags { get; set; }
    }
}