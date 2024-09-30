namespace Blog.Application.Dtos
{
    public class TagDto : BaseDto<string>
    {
        public string Name { get; set; }

        public IList<string> PostIds { get; set; }
    }
}