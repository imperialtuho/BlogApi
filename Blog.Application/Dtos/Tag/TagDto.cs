using Blog.Application.Dtos.Base;

namespace Blog.Application.Dtos.Tag
{
    public class TagDto : BaseDto
    {
        public string Id { get; set; }

        public string Label { get; set; }

        public string Slug { get; set; }

        public IList<string>? PostIds { get; set; }

        public int? PostCount => PostIds == null ? 0 : PostIds.Count;
    }
}