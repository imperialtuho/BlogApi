namespace Blog.Application.Dtos.Category
{
    public class CategoryCreateRequest
    {
        public string Title { get; set; }

        public string Label { get; set; }

        public string Description { get; set; }

        public string CoverImageUrl { get; set; }

        public string Slug { get; set; }

        public int DisplayPosition { get; set; }
    }
}