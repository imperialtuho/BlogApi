using Blog.Application.Dtos.Base;

namespace Blog.Application.Dtos.Category
{
    public class CategoryDto : BaseDto
    {
        public string CategoryId { get; set; }

        public string? Name { get; set; }

        public IList<string>? PostIds { get; set; } = [];

        public long PostCount => PostIds?.Count ?? 0;

        public string? UserId { get; set; }

        public bool IsGlobal { get; set; }
    }
}