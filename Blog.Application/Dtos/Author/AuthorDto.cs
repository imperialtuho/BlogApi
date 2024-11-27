using Blog.Application.Dtos.Base;

namespace Blog.Application.Dtos.Author
{
    public class AuthorDto : BaseDto
    {
        public string AuthorId { get; set; }

        public string Name { get; set; }
    }
}