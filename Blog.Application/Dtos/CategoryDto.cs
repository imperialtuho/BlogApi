namespace Blog.Application.Dtos
{
    public class CategoryDto : BaseDto<string>
    {
        public string CategoryId => Id;

        public string Name { get; set; }

        public long PostCount { get; set; }
    }
}