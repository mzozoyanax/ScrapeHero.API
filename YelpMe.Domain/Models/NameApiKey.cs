using System;
using System.Collections.Generic;

namespace YelpMe.Domain.Models;

public class NameApiKey : BaseEntity
{
    public string ApiKey { get; set; } = null!;
}
