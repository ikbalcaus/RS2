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

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<PublisherFollow> PublisherFollows { get; set; }

    public virtual DbSet<Purchase> Purchases { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<ReadingProgress> ReadingProgresses { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

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
            entity.HasKey(e => new { e.UserId, e.BookId }).HasName("PK__AccessRi__7456C06C617F4F44");

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.AccessRights)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AccessRig__BookI__6A30C649");

            entity.HasOne(d => d.User).WithMany(p => p.AccessRights)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AccessRig__UserI__693CA210");
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Authors__70DAFC345AD9B3BB");

            entity.HasIndex(e => e.Name, "UQ__Authors__737584F632E31123").IsUnique();

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.ModifiedBy).WithMany(p => p.Authors)
                .HasForeignKey(d => d.ModifiedById)
                .HasConstraintName("FK__Authors__Modifie__4316F928");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Books__3DE0C2076FC51366");

            entity.Property(e => e.DeletionReason).HasMaxLength(255);
            entity.Property(e => e.DiscountEnd).HasColumnType("datetime");
            entity.Property(e => e.DiscountStart).HasColumnType("datetime");
            entity.Property(e => e.FilePath).HasMaxLength(100);
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RejectionReason).HasMaxLength(255);
            entity.Property(e => e.StateMachine)
                .HasMaxLength(50)
                .HasDefaultValue("draft");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Language).WithMany(p => p.Books)
                .HasForeignKey(d => d.LanguageId)
                .HasConstraintName("FK__Books__LanguageI__52593CB8");

            entity.HasOne(d => d.Publisher).WithMany(p => p.BookPublishers)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Books__Publisher__534D60F1");

            entity.HasOne(d => d.ReviewedBy).WithMany(p => p.BookReviewedBies)
                .HasForeignKey(d => d.ReviewedById)
                .HasConstraintName("FK__Books__ReviewedB__5441852A");
        });

        modelBuilder.Entity<BookAuthor>(entity =>
        {
            entity.HasKey(e => new { e.BookId, e.AuthorId }).HasName("PK__BookAuth__6AED6DC4132FDB52");

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Author).WithMany(p => p.BookAuthors)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookAutho__Autho__5DCAEF64");

            entity.HasOne(d => d.Book).WithMany(p => p.BookAuthors)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookAutho__BookI__5CD6CB2B");
        });

        modelBuilder.Entity<BookGenre>(entity =>
        {
            entity.HasKey(e => new { e.BookId, e.GenreId }).HasName("PK__BookGenr__CDD892507C200D82");

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.BookGenres)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookGenre__BookI__5812160E");

            entity.HasOne(d => d.Genre).WithMany(p => p.BookGenres)
                .HasForeignKey(d => d.GenreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookGenre__Genre__59063A47");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK__Genres__0385057EB27E9882");

            entity.HasIndex(e => e.Name, "UQ__Genres__737584F6DA43679A").IsUnique();

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.ModifiedBy).WithMany(p => p.Genres)
                .HasForeignKey(d => d.ModifiedById)
                .HasConstraintName("FK__Genres__Modified__47DBAE45");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.LanguageId).HasName("PK__Language__B93855ABD7D24373");

            entity.HasIndex(e => e.Name, "UQ__Language__737584F65EBDF0A8").IsUnique();

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.ModifiedBy).WithMany(p => p.Languages)
                .HasForeignKey(d => d.ModifiedById)
                .HasConstraintName("FK__Languages__Modif__4CA06362");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12ED3BF5B1");

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK__Notificat__BookI__08B54D69");

            entity.HasOne(d => d.Publisher).WithMany(p => p.NotificationPublishers)
                .HasForeignKey(d => d.PublisherId)
                .HasConstraintName("FK__Notificat__Publi__09A971A2");

            entity.HasOne(d => d.User).WithMany(p => p.NotificationUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__0A9D95DB");
        });

        modelBuilder.Entity<PublisherFollow>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.PublisherId }).HasName("PK__Publishe__B34E9BB6A477B14C");

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Publisher).WithMany(p => p.PublisherFollowPublishers)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Publisher__Publi__03F0984C");

            entity.HasOne(d => d.User).WithMany(p => p.PublisherFollowUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Publisher__UserI__02FC7413");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.PurchaseId).HasName("PK__Purchase__6B0A6BBEAEE16D8E");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FailureCode).HasMaxLength(50);
            entity.Property(e => e.FailureMessage).HasMaxLength(255);
            entity.Property(e => e.FailureReason).HasMaxLength(255);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus).HasMaxLength(50);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TransactionId).HasMaxLength(255);

            entity.HasOne(d => d.Book).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Purchases__BookI__7F2BE32F");

            entity.HasOne(d => d.Publisher).WithMany(p => p.PurchasePublishers)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Purchases__Publi__7E37BEF6");

            entity.HasOne(d => d.User).WithMany(p => p.PurchaseUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Purchases__UserI__7D439ABD");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__Question__0DC06FAC2AED75BF");

            entity.Property(e => e.AnsweredAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Question1).HasColumnName("Question");

            entity.HasOne(d => d.AnsweredBy).WithMany(p => p.QuestionAnsweredBies)
                .HasForeignKey(d => d.AnsweredById)
                .HasConstraintName("FK__Questions__Answe__0F624AF8");

            entity.HasOne(d => d.User).WithMany(p => p.QuestionUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Questions__UserI__0E6E26BF");
        });

        modelBuilder.Entity<ReadingProgress>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BookId }).HasName("PK__ReadingP__7456C06C16BB0080");

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.ReadingProgresses)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ReadingPr__BookI__6EF57B66");

            entity.HasOne(d => d.User).WithMany(p => p.ReadingProgresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ReadingPr__UserI__6E01572D");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BookId }).HasName("PK__Reports__7456C06C41F69097");

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.Reports)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reports__BookId__797309D9");

            entity.HasOne(d => d.User).WithMany(p => p.Reports)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reports__UserId__787EE5A0");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BookId }).HasName("PK__Reviews__7456C06CA106DD71");

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__BookId__73BA3083");

            entity.HasOne(d => d.ReportedBy).WithMany(p => p.ReviewReportedBies)
                .HasForeignKey(d => d.ReportedById)
                .HasConstraintName("FK__Reviews__Reporte__74AE54BC");

            entity.HasOne(d => d.User).WithMany(p => p.ReviewUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__UserId__72C60C4A");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1ADC6046CA");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CD7A3B3FC");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534320D7866").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ__Users__C9F28456E34C08FF").IsUnique();

            entity.Property(e => e.DeletionReason).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FilePath).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PasswordSalt).HasMaxLength(255);
            entity.Property(e => e.StripeAccountId).HasMaxLength(100);
            entity.Property(e => e.TokenExpiry).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.VerificationToken).HasMaxLength(100);

            entity.HasOne(d => d.PublisherVerifiedBy).WithMany(p => p.InversePublisherVerifiedBy)
                .HasForeignKey(d => d.PublisherVerifiedById)
                .HasConstraintName("FK__Users__Publisher__3E52440B");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__3D5E1FD2");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BookId }).HasName("PK__Wishlist__7456C06CEFFA3EC0");

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Wishlists__BookI__628FA481");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Wishlists__UserI__619B8048");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
