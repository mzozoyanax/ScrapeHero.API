using System;
using System.Collections.Generic;

namespace YelpMe.Domain.Models;

public partial class VerifyLogger 
{
    public int Id { get; set; }

    public string? Email { get; set; }

    public bool Valid { get; set; }
}
