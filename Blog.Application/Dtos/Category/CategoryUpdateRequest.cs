using Blog.Domain.Extensions;

namespace Blog.Application.Dtos.Category
{
    public class CategoryUpdateRequest
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Label { get; set; }

        public string Description { get; set; }

        public string CoverImageUrl { get; set; }

        public string Slug => StringHelper.ToSlug(Title);

        public int DisplayPosition { get; set; }
    }
}