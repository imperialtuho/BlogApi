namespace Blog.Application.Dtos.Post
{
    public class PostCreateRequest
    {
        public int TenantId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? Url { get; set; }
        public string CategoryId { get; set; } // ID of the existing category
        public string UserId { get; set; } // ID of the existing user
        public List<string> TagIds { get; set; } // IDs of the existing tags
    }
}