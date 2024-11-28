using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class Review
{
    public int BookId { get; set; }

    public int UserId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime ReviewDate { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
