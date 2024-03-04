using System;
using System.Collections.Generic;

namespace Zest.DBModels.Models;

public partial class Follower
{
    public string FollowerId { get; set; }

    public string FollowedId { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual Account Followed { get; set; } = null!;

    public virtual Account FollowerNavigation { get; set; } = null!;
}
