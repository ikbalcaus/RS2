using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class Favorite
{
    public int UserId { get; set; }

    public int BookId { get; set; }

    public DateTime? AddedDate { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual UserRes User { get; set; } = null!;
}
