using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class Question
{
    public int QuestionId { get; set; }

    public int UserId { get; set; }

    public string Question1 { get; set; } = null!;

    public string? Answer { get; set; }

    public int? AnsweredById { get; set; }

    public DateTime ModifiedAt { get; set; }

    public DateTime? AnsweredAt { get; set; }

    public virtual User? AnsweredBy { get; set; }

    public virtual User User { get; set; } = null!;
}
