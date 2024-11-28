using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class Author
{
    public int AuthorId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Biography { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
