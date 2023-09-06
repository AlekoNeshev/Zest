using System;
using System.Collections.Generic;

namespace Zest.DBModels.Models;

public partial class Follower
{
    public int Id { get; set; }

    public int FollowerId { get; set; }

    public int FollowedId { get; set; }

    public DateTime CreatedOn { get; set; }
}
