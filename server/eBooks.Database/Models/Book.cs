using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string FilePath { get; set; } = null!;

    public decimal? Price { get; set; }

    public int? NumberOfPages { get; set; }

    public int? NumberOfViews { get; set; }

    public int LanguageId { get; set; }

    public int PublisherId { get; set; }

    public int? ReviewedById { get; set; }

    public DateTime ModifiedAt { get; set; }

    public string? StateMachine { get; set; }

    public int? DiscountPercentage { get; set; }

    public DateTime? DiscountStart { get; set; }

    public DateTime? DiscountEnd { get; set; }

    public string? RejectionReason { get; set; }

    public bool IsDeleted { get; set; }

    public string? DeleteReason { get; set; }

    public virtual ICollection<AccessRight> AccessRights { get; set; } = new List<AccessRight>();

    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

    public virtual ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual Language Language { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual User Publisher { get; set; } = null!;

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    public virtual ICollection<ReadingProgress> ReadingProgresses { get; set; } = new List<ReadingProgress>();

    public virtual User? ReviewedBy { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
