using System;
using System.Collections.Generic;

namespace Zest.DBModels.Models;

public partial class Post
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Text { get; set; } = null!;

    public int AccountId { get; set; }

    public int CommunityId { get; set; }

    public DateTime CreatedOn { get; set; }
}
