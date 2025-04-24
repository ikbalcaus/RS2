using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class PublisherFollow
{
    public int UserId { get; set; }

    public int PublisherId { get; set; }

    public DateTime? FollowDate { get; set; }

    public virtual User Publisher { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
