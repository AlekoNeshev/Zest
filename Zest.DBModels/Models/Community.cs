using System;
using System.Collections.Generic;

namespace Zest.DBModels.Models;

public partial class Community
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Information { get; set; } = null!;

    public int CreatorId { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual Account Creator { get; set; } = null!;

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
