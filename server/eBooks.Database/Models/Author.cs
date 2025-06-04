using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class Author
{
    public int AuthorId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime ModifiedAt { get; set; }

    public int? ModifiedById { get; set; }

    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

    public virtual User? ModifiedBy { get; set; }
}
