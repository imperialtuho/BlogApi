using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure.Database
{
    /// <summary>
    /// Represents the application's Entity Framework Core database context.
    /// </summary>
    /// <remarks>
    /// This class serves as the primary interface to the database, inheriting from <see cref="DbContext"/>.
    /// It defines the application's data model and exposes DbSets for each entity type that is part of the
    /// data model, enabling CRUD operations and queries via Entity Framework Core.
    /// </remarks>
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Interaction> Interactions { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<PostCategory> PostCategories { get; set; }

        public ApplicationDbContext()
        { }

        /// <summary>
        /// Initializes a new instance of the ApplicationDbContext with database options.
        /// </summary>
        /// <param name="options">Database context options.</param>
        /// <remarks>
        /// This constructor is used when configuring the DbContext with specific options, such as connection strings
        /// or other configurations required for establishing a connection to the database.
        /// </remarks>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Configures the entity model using Fluent API.
        /// </summary>
        /// <param name="modelBuilder">The model builder used to configure entity relationships.</param>
        /// <remarks>
        /// The method is used to configure the entity model using Fluent API, allowing the customization of
        /// entity relationships, table names, indexes, constraints, etc. The method uses reflection to apply
        /// configurations from the current assembly, making it easier to manage entity configurations in a modular way.
        /// </remarks>
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