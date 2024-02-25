using System;
using System.Collections.Generic;

namespace YelpMe.Domain.Models;

public partial class Setting 
{
    public int Id { get; set; }

    public bool SearchOffline { get; set; }

    public bool UseNameApi { get; set; }

    public bool FacebookPixelInstalled { get; set; }

    public bool HasYouTubeChannel { get; set; }
}
