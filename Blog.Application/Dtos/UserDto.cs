namespace Blog.Application.Dtos
{
    public class UserDto : BaseDto<string>
    {
        public string UserId => Id;

        public string Name { get; set; }

        public string Email { get; set; }
    }
}