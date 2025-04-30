using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public string? PasswordSalt { get; set; }

    public virtual ICollection<AccessRight> AccessRights { get; set; } = new List<AccessRight>();

    public virtual ICollection<BookFollow> BookFollows { get; set; } = new List<BookFollow>();

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Notification> NotificationPublishers { get; set; } = new List<Notification>();

    public virtual ICollection<Notification> NotificationUsers { get; set; } = new List<Notification>();

    public virtual ICollection<PublisherFollow> PublisherFollowPublishers { get; set; } = new List<PublisherFollow>();

    public virtual ICollection<PublisherFollow> PublisherFollowUsers { get; set; } = new List<PublisherFollow>();

    public virtual ICollection<PublisherVerification> PublisherVerificationAdmins { get; set; } = new List<PublisherVerification>();

    public virtual PublisherVerification? PublisherVerificationPublisher { get; set; }

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    public virtual ICollection<ReadingProgress> ReadingProgresses { get; set; } = new List<ReadingProgress>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<UserRole> UserRoleAssignedByNavigations { get; set; } = new List<UserRole>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
