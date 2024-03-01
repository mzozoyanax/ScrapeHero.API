using System;
using System.Collections.Generic;

namespace YelpMe.Domain.Models;

public class VerifyLogger : BaseEntity
{
    public string? Email { get; set; }

    public bool Valid { get; set; }
}
