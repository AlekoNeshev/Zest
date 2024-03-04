using System;
using System.Collections.Generic;

namespace Zest.DBModels.Models;

public partial class CommunityFollower
{
    public int CommunityId { get; set; }

    public string AccountId { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Community Community { get; set; } = null!;
}
