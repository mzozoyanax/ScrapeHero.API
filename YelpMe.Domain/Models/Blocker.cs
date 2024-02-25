using System;
using System.Collections.Generic;

namespace YelpMe.Domain.Models;

public class Blocker
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public DateTime AddedDate { get; set; }
}
