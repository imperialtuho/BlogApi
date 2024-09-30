namespace Blog.Application.Dtos
{
    public class CategoryDto : BaseDto<string>
    {
        public string CategoryId => Id;

        public string? Name { get; set; }

        public IList<string?> PostIds { get; set; } = [];

        public long PostCount => PostIds.Count;
    }
}