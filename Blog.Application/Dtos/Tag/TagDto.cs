using Blog.Application.Dtos.Base;

namespace Blog.Application.Dtos.Tag
{
    public class TagDto : BaseDto
    {
        public string Name { get; set; }

        public IList<string> PostIds { get; set; }
    }
}