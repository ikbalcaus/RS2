using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using eBooks.Database.Models;

namespace eBooks.Database;

public partial class EBooksContext : DbContext
{
    public EBooksContext()
    {
    }

    public EBooksContext(DbContextOptions<EBooksContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccessRight> AccessRights { get; set; }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookAuthor> BookAuthors { get; set; }

    public virtual DbSet<BookGenre> BookGenres { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<PublisherFollow> PublisherFollows { get; set; }

    public virtual DbSet<Purchase> Purchases { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<ReadingProgress> ReadingProgresses { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=Database");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccessRight>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BookId }).HasName("PK__AccessRi__7456C06C4B8DC486");

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.AccessRights)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AccessRig__BookI__6C190EBB");

            entity.HasOne(d => d.User).WithMany(p => p.AccessRights)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AccessRig__UserI__6B24EA82");
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Authors__70DAFC34447B1E9B");

            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Books__3DE0C207479E280A");

            entity.Property(e => e.DeleteReason).HasMaxLength(255);
            entity.Property(e => e.DiscountEnd).HasColumnType("datetime");
            entity.Property(e => e.DiscountStart).HasColumnType("datetime");
            entity.Property(e => e.FilePath).HasMaxLength(255);
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
            entity.Property(e => e.StateMachine)
                .HasMaxLength(50)
                .HasDefaultValue("draft");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Language).WithMany(p => p.Books)
                .HasForeignKey(d => d.LanguageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Books__LanguageI__4D94879B");

            entity.HasOne(d => d.Publisher).WithMany(p => p.BookPublishers)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Books__Publisher__4E88ABD4");

            entity.HasOne(d => d.ReviewedBy).WithMany(p => p.BookReviewedBies)
                .HasForeignKey(d => d.ReviewedById)
                .HasConstraintName("FK__Books__ReviewedB__4F7CD00D");
        });

        modelBuilder.Entity<BookAuthor>(entity =>
        {
            entity.HasKey(e => new { e.BookId, e.AuthorId }).HasName("PK__BookAuth__6AED6DC494DC0C78");

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Author).WithMany(p => p.BookAuthors)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookAutho__Autho__59063A47");

            entity.HasOne(d => d.Book).WithMany(p => p.BookAuthors)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookAutho__BookI__5812160E");
        });

        modelBuilder.Entity<BookGenre>(entity =>
        {
            entity.HasKey(e => new { e.BookId, e.GenreId }).HasName("PK__BookGenr__CDD892508E1F1F30");

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.BookGenres)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookGenre__BookI__534D60F1");

            entity.HasOne(d => d.Genre).WithMany(p => p.BookGenres)
                .HasForeignKey(d => d.GenreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookGenre__Genre__5441852A");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BookId }).HasName("PK__Favorite__7456C06C53230555");

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Favorites__BookI__619B8048");

            entity.HasOne(d => d.User).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Favorites__UserI__60A75C0F");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK__Genres__0385057ED9C1336B");

            entity.HasIndex(e => e.Name, "UQ__Genres__737584F6863ACA62").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.LanguageId).HasName("PK__Language__B93855AB907B13D4");

            entity.HasIndex(e => e.Name, "UQ__Language__737584F6956C9F23").IsUnique();

            entity.HasIndex(e => e.Abbreviation, "UQ__Language__9C41170E9F8E2714").IsUnique();

            entity.Property(e => e.Abbreviation).HasMaxLength(10);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E1244E04D66");

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK__Notificat__BookI__04E4BC85");

            entity.HasOne(d => d.Publisher).WithMany(p => p.NotificationPublishers)
                .HasForeignKey(d => d.PublisherId)
                .HasConstraintName("FK__Notificat__Publi__05D8E0BE");

            entity.HasOne(d => d.User).WithMany(p => p.NotificationUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__06CD04F7");
        });

        modelBuilder.Entity<PublisherFollow>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.PublisherId }).HasName("PK__Publishe__B34E9BB618798F37");

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Publisher).WithMany(p => p.PublisherFollowPublishers)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Publisher__Publi__7B5B524B");

            entity.HasOne(d => d.User).WithMany(p => p.PublisherFollowUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Publisher__UserI__7A672E12");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.PurchaseId).HasName("PK__Purchase__6B0A6BBEFC449DA1");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FailureCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FailureMessage)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FailureReason)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Book).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Purchases_Book");

            entity.HasOne(d => d.Publisher).WithMany(p => p.PurchasePublishers)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Purchases_Publisher");

            entity.HasOne(d => d.User).WithMany(p => p.PurchaseUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Purchases_User");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__Question__0DC06FACFC16BE18");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Question1).HasColumnName("Question");

            entity.HasOne(d => d.AnsweredBy).WithMany(p => p.QuestionAnsweredBies)
                .HasForeignKey(d => d.AnsweredById)
                .HasConstraintName("FK__Questions__Answe__0B91BA14");

            entity.HasOne(d => d.User).WithMany(p => p.QuestionUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Questions__UserI__0A9D95DB");
        });

        modelBuilder.Entity<ReadingProgress>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BookId }).HasName("PK__ReadingP__7456C06C0BFC2661");

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.ReadingProgresses)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ReadingPr__BookI__70DDC3D8");

            entity.HasOne(d => d.User).WithMany(p => p.ReadingProgresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ReadingPr__UserI__6FE99F9F");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BookId }).HasName("PK__Reviews__7456C06CB34E448E");

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__BookId__00200768");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__UserId__7F2BE32F");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1AA5388ECC");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C47BFFDF4");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105340A920177").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ__Users__C9F2845694262C97").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeleteReason).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PasswordSalt).HasMaxLength(255);
            entity.Property(e => e.StripeAccountId).HasMaxLength(255);
            entity.Property(e => e.TokenExpiry).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.VerificationToken).HasMaxLength(100);

            entity.HasOne(d => d.PublisherVerifiedBy).WithMany(p => p.InversePublisherVerifiedBy)
                .HasForeignKey(d => d.PublisherVerifiedById)
                .HasConstraintName("FK__Users__Publisher__3F466844");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__3E52440B");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BookId }).HasName("PK__Wishlist__7456C06C68413AA3");

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Wishlists__BookI__66603565");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Wishlists__UserI__656C112C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
