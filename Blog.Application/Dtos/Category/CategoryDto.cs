using Blog.Application.Dtos.Author;
using Blog.Application.Dtos.Base;
using Blog.Application.Dtos.Media;

namespace Blog.Application.Dtos.Category
{
    public class CategoryDto : BaseDto
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Label { get; set; }

        public string? Description { get; set; }

        public IList<MediaDto>? Media { get; set; }

        public string Slug { get; set; }

        public int DisplayPosition { get; set; }

        public long PostCount { get; set; }

        public AuthorDto? Author { get; set; }
    }
}