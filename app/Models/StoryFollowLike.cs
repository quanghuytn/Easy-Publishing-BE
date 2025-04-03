﻿using System;
using System.Collections.Generic;

namespace app.Models;

public partial class StoryFollowLike
{
    public int UserId { get; set; }

    public int StoryId { get; set; }

    public bool? Follow { get; set; }

    public bool? Like { get; set; }

    public virtual Story Story { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
