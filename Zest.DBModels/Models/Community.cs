using System;
using System.Collections.Generic;

namespace Zest.DBModels.Models;

public partial class Community
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Information { get; set; } = null!;

    public DateTime CreatedOn { get; set; }
}
