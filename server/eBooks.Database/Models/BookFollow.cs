using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class BookFollow
{
    public int UserId { get; set; }

    public int BookId { get; set; }

    public DateTime FollowDate { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
