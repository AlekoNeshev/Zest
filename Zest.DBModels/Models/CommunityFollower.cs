using System;
using System.Collections.Generic;

namespace Zest.DBModels.Models;

public partial class CommunityFollower
{
    public int Id { get; set; }

    public int CommunityId { get; set; }

    public int AccountId { get; set; }

    public DateTime CreatedOn { get; set; }
}
