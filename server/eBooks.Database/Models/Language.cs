using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class Language
{
    public int LanguageId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime ModifiedAt { get; set; }

    public int? ModifiedById { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public virtual User? ModifiedBy { get; set; }
}
