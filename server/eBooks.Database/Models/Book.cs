using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string? Title { get; set; }

    public string? PdfPath { get; set; }

    public decimal? Price { get; set; }

    public int PublisherId { get; set; }

    public DateTime? AddedDate { get; set; }

    public string? StateMachine { get; set; }

    public string? RejectionReason { get; set; }

    public virtual ICollection<AccessRight> AccessRights { get; set; } = new List<AccessRight>();

    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

    public virtual ICollection<BookFollow> BookFollows { get; set; } = new List<BookFollow>();

    public virtual ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();

    public virtual ICollection<BookImage> BookImages { get; set; } = new List<BookImage>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual UserRes Publisher { get; set; } = null!;

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    public virtual ICollection<ReadingProgress> ReadingProgresses { get; set; } = new List<ReadingProgress>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
