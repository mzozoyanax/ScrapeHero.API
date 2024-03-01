using System;
using System.Collections.Generic;

namespace YelpMe.Domain.Models;

public class Blocker : BaseEntity
{
    public string Email { get; set; } = null!;
}
