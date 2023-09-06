using System;
using System.Collections.Generic;

namespace Zest.DBModels.Models;

public partial class Message
{
    public int Id { get; set; }

    public string Text { get; set; } = null!;

    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public DateTime CreatedOn { get; set; }
}
