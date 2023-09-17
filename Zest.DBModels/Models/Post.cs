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

    public virtual Account Account { get; set; } = null!;

    public virtual Community Community { get; set; } = null!;

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
