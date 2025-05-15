using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class BookGenre
{
    public int BookId { get; set; }

    public int GenreId { get; set; }

    public DateTime ModifiedAt { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Genre Genre { get; set; } = null!;
}
