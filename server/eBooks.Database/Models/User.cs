using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public DateTime ModifiedAt { get; set; }

    public bool IsEmailVerified { get; set; }

    public string? VerificationToken { get; set; }

    public DateTime? TokenExpiry { get; set; }

    public string? DeletionReason { get; set; }

    public string? StripeAccountId { get; set; }

    public int RoleId { get; set; }

    public int? PublisherVerifiedById { get; set; }

    public virtual ICollection<AccessRight> AccessRights { get; set; } = new List<AccessRight>();

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();

    public virtual ICollection<Book> BookPublishers { get; set; } = new List<Book>();

    public virtual ICollection<Book> BookReviewedBies { get; set; } = new List<Book>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();

    public virtual ICollection<User> InversePublisherVerifiedBy { get; set; } = new List<User>();

    public virtual ICollection<Language> Languages { get; set; } = new List<Language>();

    public virtual ICollection<Notification> NotificationPublishers { get; set; } = new List<Notification>();

    public virtual ICollection<Notification> NotificationUsers { get; set; } = new List<Notification>();

    public virtual ICollection<PublisherFollow> PublisherFollowPublishers { get; set; } = new List<PublisherFollow>();

    public virtual ICollection<PublisherFollow> PublisherFollowUsers { get; set; } = new List<PublisherFollow>();

    public virtual User? PublisherVerifiedBy { get; set; }

    public virtual ICollection<Purchase> PurchasePublishers { get; set; } = new List<Purchase>();

    public virtual ICollection<Purchase> PurchaseUsers { get; set; } = new List<Purchase>();

    public virtual ICollection<Question> QuestionAnsweredBies { get; set; } = new List<Question>();

    public virtual ICollection<Question> QuestionUsers { get; set; } = new List<Question>();

    public virtual ICollection<ReadingProgress> ReadingProgresses { get; set; } = new List<ReadingProgress>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
