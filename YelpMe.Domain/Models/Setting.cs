using System;
using System.Collections.Generic;

namespace YelpMe.Domain.Models;

public class Setting : BaseEntity
{
    public bool SearchOffline { get; set; }

    public bool UseNameApi { get; set; }

    public bool FacebookPixelInstalled { get; set; }

    public bool HasYouTubeChannel { get; set; }
}
