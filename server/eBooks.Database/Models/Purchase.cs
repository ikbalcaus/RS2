using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class Purchase
{
    public int PurchaseId { get; set; }

    public int UserId { get; set; }

    public DateTime PurchaseDate { get; set; }

    public decimal TotalPrice { get; set; }

    public virtual ICollection<AccessRight> AccessRights { get; set; } = new List<AccessRight>();

    public virtual User User { get; set; } = null!;
}
