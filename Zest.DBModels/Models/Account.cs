using System;
using System.Collections.Generic;

namespace Zest.DBModels.Models;

public partial class Account
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Username { get; set; } = null!;

    public DateTime Birthdate { get; set; }

    public DateTime CreatedOn { get; set; }
}
