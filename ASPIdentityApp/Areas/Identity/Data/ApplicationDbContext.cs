using ASPIdentityApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASPIdentityApp.Areas.Identity.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    // Add DbSet properties for your entities here
    // DbSets for Library Management
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<BookIssue> BookIssues { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)  // add Modification related properties here
    { 
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        // Configure ApplicationUser properties
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(255).IsRequired();
        });
        // Optional: Customize table names if needed
        // builder.Entity<ApplicationUser>().ToTable("Users");
        // builder.Entity<IdentityRole>().ToTable("Roles");
        // builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        // builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        // builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        // builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        // builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

        // Book configuration
        builder.Entity<Book>(entity =>
        {
            entity.HasIndex(e => e.ISBN).IsUnique();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ISBN).IsRequired().HasMaxLength(13);
            entity.Property(e => e.Genre).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);

            // Configure relationship with Author
            entity.HasOne(b => b.Author)
                  .WithMany(a => a.Books)
                  .HasForeignKey(b => b.AuthorId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Publisher
            entity.HasOne(b => b.Publisher)
                  .WithMany(p => p.Books)
                  .HasForeignKey(b => b.PublisherId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Author configuration
        builder.Entity<Author>(entity =>
        {
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(255);

            // Configure the relationship to be optional
            //entity.HasMany(a => a.Books)
            //      .WithOne(b => b.Author)
            //      .HasForeignKey(b => b.AuthorId)
            //      .OnDelete(DeleteBehavior.ClientSetNull); // Or use DeleteBehavior.SetNull
        });

        // Publisher configuration
        builder.Entity<Publisher>(entity =>
        {
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(255);
        });

        // Member configuration - UPDATED for new Member model
        builder.Entity<Member>(entity =>
        {
            entity.HasIndex(e => e.MemberCode).IsUnique(); // Changed from MemberId to MemberCode
            entity.Property(e => e.MemberCode).IsRequired().HasMaxLength(20);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            entity.Property(e => e.TotalFees).HasPrecision(18, 2);
        });

        // BookIssue configuration - CORRECTED
        builder.Entity<BookIssue>(entity =>
        {
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            entity.Property(e => e.FineAmount).HasPrecision(18, 2);
            entity.Property(e => e.Notes).HasMaxLength(500);

            // Configure relationship with Book - CORRECTED
            entity.HasOne(bi => bi.Book)
                  .WithMany(b => b.BookIssues) // This was missing - causes the BookId1 error
                  .HasForeignKey(bi => bi.BookId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with Member - ADD THIS
            entity.HasOne(bi => bi.Member)
                  .WithMany(m => m.BookIssues) // This was missing
                  .HasForeignKey(bi => bi.MemberId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
