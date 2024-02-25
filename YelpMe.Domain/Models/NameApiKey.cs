using System;
using System.Collections.Generic;

namespace YelpMe.Domain.Models;

public partial class NameApiKey 
{
    public int Id { get; set; }

    public string ApiKey { get; set; } = null!;
}
