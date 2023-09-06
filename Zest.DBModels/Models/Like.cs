﻿using System;
using System.Collections.Generic;

namespace Zest.DBModels.Models;

public partial class Like
{
    public int Id { get; set; }

    public bool Value { get; set; }

    public int AccountId { get; set; }

    public int? PostId { get; set; }

    public int? CommentId { get; set; }

    public DateTime CreatedOn { get; set; }
}