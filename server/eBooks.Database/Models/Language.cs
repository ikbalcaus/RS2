using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class Language
{
    public int LanguageId { get; set; }

    public string Name { get; set; } = null!;

    public string Abbreviation { get; set; } = null!;
}
