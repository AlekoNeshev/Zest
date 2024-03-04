using System;
using System.Collections.Generic;

namespace Zest.DBModels.Models;

public partial class Like
{
    public int Id { get; set; }

    public bool Value { get; set; }

    public string AccountId { get; set; }

    public int? PostId { get; set; }

    public int? CommentId { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Comment? Comment { get; set; }

    public virtual Post? Post { get; set; }
}
