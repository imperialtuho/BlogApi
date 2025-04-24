using Blog.Domain.Enums;
using Blog.Domain.Extensions;
using Blog.Domain.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Blog.Domain.Entities
{
    public class Post : BaseEntity<string>
    {
        public string Title { get; set; }

        [MaxLength(500)]
        public string Summary { get; set; }

        [MaxLength(400000)]
        public string Content { get; set; }

        public bool Featured { get; set; }

        public bool Pinned { get; set; }

        public bool CommentingEnabled { get; set; }

        public string Status { get; set; } = nameof(PostStatus.Unknown);

        public IList<string>? HashTags { get; set; }

        public string? CoverImageUrl { get; set; }

        // Foreign Keys
        public string AuthorId { get; set; }

        // Navigation Properties
        public ICollection<PostCategory>? PostCategories { get; set; }

        public ICollection<PostTag>? PostTags { get; set; }

        public ICollection<Comment>? Comments { get; set; }

        public ICollection<Interaction>? Interactions { get; set; }
    }
}