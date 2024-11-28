using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class AuthToken
{
    public int AuthTokenId { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime LoggedInDate { get; set; }

    public DateTime ExpirationDate { get; set; }

    public DateTime? LoggedOutDate { get; set; }

    public virtual User User { get; set; } = null!;
}
