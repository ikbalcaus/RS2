using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class Genre
{
    public int GenreId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime ModifiedAt { get; set; }

    public int? ModifiedById { get; set; }

    public virtual ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();

    public virtual User? ModifiedBy { get; set; }
}
