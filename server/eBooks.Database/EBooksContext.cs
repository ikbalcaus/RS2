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

    public virtual DbSet<BookFollow> BookFollows { get; set; }

    public virtual DbSet<BookGenre> BookGenres { get; set; }

    public virtual DbSet<BookImage> BookImages { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<PublisherFollow> PublisherFollows { get; set; }

    public virtual DbSet<PublisherVerification> PublisherVerifications { get; set; }

    public virtual DbSet<Purchase> Purchases { get; set; }

    public virtual DbSet<ReadingProgress> ReadingProgresses { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=Database");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccessRight>(entity =>
        {
            entity.HasKey(e => e.AccessRightId).HasName("PK__AccessRi__4E70724E3D47D3C1");

            entity.HasOne(d => d.Book).WithMany(p => p.AccessRights)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AccessRig__BookI__7A672E12");

            entity.HasOne(d => d.User).WithMany(p => p.AccessRights)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AccessRig__UserI__797309D9");
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Authors__70DAFC3448372BDE");

            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Books__3DE0C207F3D168ED");

            entity.Property(e => e.AddedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("isDeleted");
            entity.Property(e => e.PdfPath).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
            entity.Property(e => e.StateMachine)
                .HasMaxLength(50)
                .HasDefaultValue("draft");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Publisher).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Books__Publisher__47DBAE45");
        });

        modelBuilder.Entity<BookAuthor>(entity =>
        {
            entity.HasKey(e => new { e.BookId, e.AuthorId }).HasName("PK__BookAuth__6AED6DC46070CE1C");

            entity.Property(e => e.IsPrimary).HasDefaultValue(false);

            entity.HasOne(d => d.Author).WithMany(p => p.BookAuthors)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookAutho__Autho__5165187F");

            entity.HasOne(d => d.Book).WithMany(p => p.BookAuthors)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookAutho__BookI__5070F446");
        });

        modelBuilder.Entity<BookFollow>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BookId }).HasName("PK__BookFoll__7456C06C4B9172F4");

            entity.Property(e => e.FollowDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.BookFollows)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookFollo__BookI__6D0D32F4");

            entity.HasOne(d => d.User).WithMany(p => p.BookFollows)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookFollo__UserI__6C190EBB");
        });

        modelBuilder.Entity<BookGenre>(entity =>
        {
            entity.HasKey(e => new { e.BookId, e.GenreId }).HasName("PK__BookGenr__CDD8925098BA120A");

            entity.Property(e => e.IsPrimary).HasDefaultValue(false);

            entity.HasOne(d => d.Book).WithMany(p => p.BookGenres)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookGenre__BookI__4BAC3F29");

            entity.HasOne(d => d.Genre).WithMany(p => p.BookGenres)
                .HasForeignKey(d => d.GenreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookGenre__Genre__4CA06362");
        });

        modelBuilder.Entity<BookImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__BookImag__7516F70C0049046B");

            entity.Property(e => e.AddedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImagePath).HasMaxLength(255);

            entity.HasOne(d => d.Book).WithMany(p => p.BookImages)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK__BookImage__BookI__5535A963");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BookId }).HasName("PK__Favorite__7456C06C6D927E82");

            entity.Property(e => e.AddedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Favorites__BookI__59FA5E80");

            entity.HasOne(d => d.User).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Favorites__UserI__59063A47");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK__Genres__0385057E82A45816");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12118DCFA2");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);

            entity.HasOne(d => d.Book).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK__Notificat__BookI__7F2BE32F");

            entity.HasOne(d => d.Publisher).WithMany(p => p.NotificationPublishers)
                .HasForeignKey(d => d.PublisherId)
                .HasConstraintName("FK__Notificat__Publi__00200768");

            entity.HasOne(d => d.User).WithMany(p => p.NotificationUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__01142BA1");
        });

        modelBuilder.Entity<PublisherFollow>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.PublisherId }).HasName("PK__Publishe__B34E9BB6A56E57A7");

            entity.Property(e => e.FollowDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Publisher).WithMany(p => p.PublisherFollowPublishers)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Publisher__Publi__71D1E811");

            entity.HasOne(d => d.User).WithMany(p => p.PublisherFollowUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Publisher__UserI__70DDC3D8");
        });

        modelBuilder.Entity<PublisherVerification>(entity =>
        {
            entity.HasKey(e => e.VerificationId).HasName("PK__Publishe__306D49071B486A64");

            entity.ToTable("PublisherVerification");

            entity.HasIndex(e => e.PublisherId, "UQ__Publishe__4C657FAAD8A43FF5").IsUnique();

            entity.Property(e => e.VerificationDate).HasColumnType("datetime");

            entity.HasOne(d => d.Admin).WithMany(p => p.PublisherVerificationAdmins)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Publisher__Admin__05D8E0BE");

            entity.HasOne(d => d.Publisher).WithOne(p => p.PublisherVerificationPublisher)
                .HasForeignKey<PublisherVerification>(d => d.PublisherId)
                .HasConstraintName("FK__Publisher__Publi__04E4BC85");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BookId }).HasName("PK__Purchase__7456C06C9C10ECCD");

            entity.Property(e => e.PurchaseDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Book).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Purchases__BookI__6383C8BA");

            entity.HasOne(d => d.User).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Purchases__UserI__628FA481");
        });

        modelBuilder.Entity<ReadingProgress>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BookId }).HasName("PK__ReadingP__7456C06CFC3DC09A");

            entity.Property(e => e.LastReadDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.ReadingProgresses)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ReadingPr__BookI__68487DD7");

            entity.HasOne(d => d.User).WithMany(p => p.ReadingProgresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ReadingPr__UserI__6754599E");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => new { e.BookId, e.UserId }).HasName("PK__Reviews__EC984EC3B1C980C2");

            entity.Property(e => e.ReviewDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__BookId__75A278F5");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__UserId__76969D2E");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A689EEE44");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C26366786");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105341BBCDA6C").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PasswordSalt).HasMaxLength(255);
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId }).HasName("PK__UserRole__AF2760AD54AB14EB");

            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserRoles__RoleI__3F466844");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserRoles__UserI__3E52440B");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.BookId }).HasName("PK__Wishlist__7456C06C3192FA7A");

            entity.Property(e => e.AddedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Book).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Wishlists__BookI__5EBF139D");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Wishlists__UserI__5DCAEF64");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
