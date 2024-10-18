namespace Blog.Application.Dtos.Category
{
    public class CategoryCreateRequest
    {
        public string? Name { get; set; }

        public IList<string?> PostIds { get; set; } = [];

        public int? TenantId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        public bool IsActive { get; set; }

        public bool IsLocked { get; set; }
    }
}