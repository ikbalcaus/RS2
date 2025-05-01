using System;
using System.Collections.Generic;

namespace eBooks.Database.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
