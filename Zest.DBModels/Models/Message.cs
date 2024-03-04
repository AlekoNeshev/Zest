using System;
using System.Collections.Generic;

namespace Zest.DBModels.Models;

public partial class Message
{
    public int Id { get; set; }

    public string Text { get; set; } = null!;

    public string SenderId { get; set; }

    public string ReceiverId { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual Account Receiver { get; set; } = null!;

    public virtual Account Sender { get; set; } = null!;
}
