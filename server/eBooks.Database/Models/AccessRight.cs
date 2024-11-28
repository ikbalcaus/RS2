using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class AccessRight
{
    public int AccessRightId { get; set; }

    public int BookId { get; set; }

    public int MinimumAge { get; set; }

    public virtual Book Book { get; set; } = null!;
}
