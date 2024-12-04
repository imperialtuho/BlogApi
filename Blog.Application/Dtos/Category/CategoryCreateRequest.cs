using Blog.Application.Dtos.Media;

namespace Blog.Application.Dtos.Category
{
    public class CategoryCreateRequest
    {
        public string Title { get; set; }

        public string Label { get; set; }

        public string Description { get; set; }

        public IList<MediaRequest>? Media { get; set; }

        public int DisplayPosition { get; set; }
    }
}