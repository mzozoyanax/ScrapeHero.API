using System;
using System.Collections.Generic;

namespace YelpMe.Domain.Models;

public class WhatsAppIntergation : BaseEntity
{
    public string InstanceId { get; set; } = null!;

    public string ApiTokenInstance { get; set; } = null!;
}
