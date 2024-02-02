using System;
using System.Collections.Generic;

namespace Zest.DBModels.Models;

public partial class Comment
{
    public int Id { get; set; }

    public string Text { get; set; } = null!;

    public int AccountId { get; set; }

    public int PostId { get; set; }

    public int? CommentId { get; set; }
    public bool? IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Comment? CommentNavigation { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
}
