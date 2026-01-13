using Microsoft.EntityFrameworkCore;
using Neksara.Models;

namespace Neksara.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<TopicView> TopicViews { get; set; }
        public DbSet<SearchLog> SearchLogs { get; set; }    
        public DbSet<ArchiveTopic> ArchiveTopics { get; set; }
      public DbSet<ArchiveCategory> ArchiveCategories { get; set; }
        public DbSet<Testimoni> Testimonis { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== USERS =====
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.Password)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.PhotoUrl)
                      .HasMaxLength(255);

                entity.Property(e => e.Role)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");
            });

            // ===== CATEGORIES =====
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.HasKey(e => e.CategoryId);

                entity.Property(e => e.CategoryName)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.CategoryPicture)
                      .HasMaxLength(255);
                entity.Property(e => e.Description)
                      .HasMaxLength(255);

                entity.Property(e => e.IsDeleted)
                      .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");
            });

            // ===== TOPICS =====
            modelBuilder.Entity<Topic>(entity =>
            {
                entity.ToTable("Topics");

                entity.HasKey(e => e.TopicId);

                entity.Property(e => e.TopicName)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.Description)
                      .IsRequired();

                entity.Property(e => e.TopicPicture)
                      .HasMaxLength(255);

                entity.Property(e => e.VideoUrl)
                      .HasMaxLength(255);

                entity.Property(e => e.ViewCount)
                      .HasDefaultValue(0);

                entity.Property(e => e.IsDeleted)
                      .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Topics)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== TOPIC VIEW =====
            modelBuilder.Entity<TopicView>(entity =>
            {
                entity.ToTable("TopicView");

                entity.HasKey(e => e.TopicViewId);

                entity.Property(e => e.ViewedAt)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.User)
                      .WithMany(u => u.TopicViews)
                      .HasForeignKey(e => e.IdUser);

                entity.HasOne(e => e.Topic)
                      .WithMany(t => t.TopicViews)
                      .HasForeignKey(e => e.TopicId);
            });

            // ===== SEARCH LOG =====
            modelBuilder.Entity<SearchLog>(entity =>
            {
                entity.ToTable("SearchLogs");

                entity.HasKey(e => e.SearchLogId);

                entity.Property(e => e.SearchQuery)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.SearchAt)
                      .HasDefaultValueSql("GETDATE()");
            });

            // ===== FEEDBACK =====
            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("Feedbacks");

                entity.HasKey(e => e.FeedbackId);

                entity.Property(e => e.TargetId)
                      .IsRequired();    

             entity.Property(e => e.TargetType)
                      .IsRequired()
                      .HasMaxLength(50);

                 entity.Property(e => e.Name)
                      .IsRequired()

                      .HasMaxLength(100);
                entity.Property(e => e.Role)
                      .IsRequired()
                      .HasMaxLength(50);
                      
                entity.Property(e => e.Description)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(e => e.Rating)
                      .HasDefaultValue(0);

                entity.Property(e => e.IsApproved)
                      .HasDefaultValue(false);

                entity.Property(e => e.IsVisible)
                      .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.User)
                      .WithMany(u => u.Feedbacks)
                      .HasForeignKey(e => e.UserId);

            });

            // ===== ARCHIVE TOPIC =====
            modelBuilder.Entity<ArchiveTopic>(entity =>
            {
                  entity.ToTable("ArchiveTopics");

                  entity.HasKey(e => e.ArchiveTopicId);

                  entity.Property(e => e.TopicName)
                        .IsRequired()
                        .HasMaxLength(150);

                  entity.Property(e => e.Description)
                        .IsRequired();

                  entity.Property(e => e.TopicPicture)
                      .HasMaxLength(255);

                  entity.Property(e => e.CategoryPicture)
                      .HasMaxLength(255);

                  entity.Property(e => e.VideoUrl)
                        .HasMaxLength(255);

                  entity.Property(e => e.ViewCount)
                        .HasDefaultValue(0);

                  entity.Property(e => e.ArchivedAt)
                        .HasDefaultValueSql("GETDATE()");

                  // ArchiveTopic no longer has a navigation FK to live Category;
                  // keep CategoryId as a nullable snapshot column and do not configure a foreign key.
                  });

                  // ===== ARCHIVE CATEGORY =====
                  modelBuilder.Entity<ArchiveCategory>(entity =>
                  {
                      entity.ToTable("ArchiveCategories");

                      entity.HasKey(e => e.ArchiveCategoryId);

                      entity.Property(e => e.CategoryName)
                            .IsRequired()
                            .HasMaxLength(100);

                      entity.Property(e => e.Description)
                            .HasMaxLength(255);

                      entity.Property(e => e.CategoryPicture)
                            .HasMaxLength(255);

                      entity.Property(e => e.ArchivedAt)
                            .HasDefaultValueSql("GETDATE()");
                  });

                modelBuilder.Entity<Testimoni>(entity =>
                 {
                 entity.HasKey(cm => cm.TestimoniId);

                 entity.Property(cm => cm.TestimoniName)
                        .IsRequired()
                        .HasMaxLength(100);

                 entity.Property(e => e.Description)
                      .IsRequired()
                      .HasMaxLength(500);

                 entity.Property(e => e.TestimoniRating)
                        .HasDefaultValue(0);

                entity.Property(e => e.IsApproved)
                      .HasDefaultValue(false);

                entity.Property(e => e.IsVisible)
                      .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");
            });
        }
    }
}