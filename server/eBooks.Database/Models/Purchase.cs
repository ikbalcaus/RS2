using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class Purchase
{
    public int PurchaseId { get; set; }

    public int UserId { get; set; }

    public int PublisherId { get; set; }

    public int BookId { get; set; }

    public DateTime PurchaseDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public string? PaymentMethod { get; set; }

    public string? TransactionId { get; set; }

    public string? FailureMessage { get; set; }

    public string? FailureCode { get; set; }

    public string? FailureReason { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User Publisher { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
