using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public int? BookId { get; set; }

    public int? PublisherId { get; set; }

    public string Message { get; set; } = null!;

    public bool? IsRead { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Book? Book { get; set; }

    public virtual User? Publisher { get; set; }

    public virtual User User { get; set; } = null!;
}
