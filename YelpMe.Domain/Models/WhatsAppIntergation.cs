using System;
using System.Collections.Generic;

namespace YelpMe.Domain.Models;

public partial class WhatsAppIntergation 
{
    public int Id { get; set; }

    public string InstanceId { get; set; } = null!;

    public string ApiTokenInstance { get; set; } = null!;
}
