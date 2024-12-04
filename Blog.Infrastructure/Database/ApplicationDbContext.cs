using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Interaction> Interactions { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<PostCategory> PostCategories { get; set; }

        public ApplicationDbContext()
        { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Many-to-Many
            modelBuilder.Entity<PostCategory>()
                .HasKey(pc => new { pc.PostId, pc.CategoryId }); // Composite Key

            modelBuilder.Entity<PostCategory>()
                .HasOne(pc => pc.Post)
                .WithMany(p => p.PostCategories)
                .HasForeignKey(pc => pc.PostId);

            modelBuilder.Entity<PostCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.PostCategories)
                .HasForeignKey(pc => pc.CategoryId);

            // Post to Tags (Many-to-Many)
            modelBuilder.Entity<PostTag>()
                .HasKey(pt => new { pt.PostId, pt.TagId });
            modelBuilder.Entity<PostTag>()
                .HasOne(pt => pt.Post)
                .WithMany(p => p.PostTags)
                .HasForeignKey(pt => pt.PostId);
            modelBuilder.Entity<PostTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.PostTags)
                .HasForeignKey(pt => pt.TagId);

            // One-to-many relationship between Post and Comment
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId);

            // One-to-many relationship between Post and Interaction
            modelBuilder.Entity<Interaction>()
                .HasOne(i => i.Post)
                .WithMany(p => p.Interactions)
                .HasForeignKey(i => i.PostId);

            // Default Query Filter
            modelBuilder.Entity<Post>()
                .HasQueryFilter(post => !post.IsDeleted);

            modelBuilder.Entity<Category>()
                .HasQueryFilter(category => !category.IsDeleted);
        }
    }
}