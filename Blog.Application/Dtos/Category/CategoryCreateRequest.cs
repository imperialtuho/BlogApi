namespace Blog.Application.Dtos.Category
{
    public class CategoryCreateRequest
    {
        public string Title { get; set; }

        public string Label { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public int DisplayPosition { get; set; }
    }
}