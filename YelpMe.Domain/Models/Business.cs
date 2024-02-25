using System;
using System.Collections.Generic;

namespace YelpMe.Domain.Models;

public class Business 
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Profile { get; set; }

    public string? Website { get; set; }

    public string? FacebookPage { get; set; }

    public string? Instagram { get; set; }

    public string? Twitter { get; set; }

    public string? LinkedIn { get; set; }

    public string? YouTubeChannel { get; set; }

    public string? Company { get; set; }

    public string? Location { get; set; }

    public string? Keywords { get; set; }

    public string? PersonalLine { get; set; }

    public bool Sent { get; set; }

    public DateTime AddedDate { get; set; }
}
