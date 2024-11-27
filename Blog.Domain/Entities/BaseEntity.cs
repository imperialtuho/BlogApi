using System.ComponentModel.DataAnnotations;

namespace Blog.Domain.Entities
{
    public class BaseEntity<TId>
    {
        [Key]
        public TId Id { get; set; }

        public int? TenantId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; }
    }
}