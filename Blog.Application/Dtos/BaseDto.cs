namespace Blog.Application.Dtos
{
    public class BaseDto<T>
    {
        public T Id { get; set; }

        public int? TenantId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        public bool IsActive { get; set; }
    }
}