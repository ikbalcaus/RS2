using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class AccessRight
{
    public int UserId { get; set; }

    public int BookId { get; set; }

    public DateTime ModifiedAt { get; set; }

    public bool IsFavorite { get; set; }

    public bool IsHidden { get; set; }

    public int LastReadPage { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
