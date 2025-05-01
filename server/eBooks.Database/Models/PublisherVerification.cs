using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class PublisherVerification
{
    public int VerificationId { get; set; }

    public int? PublisherId { get; set; }

    public int AdminId { get; set; }

    public DateTime? VerificationDate { get; set; }

    public virtual UserRes Admin { get; set; } = null!;

    public virtual UserRes? Publisher { get; set; }
}
