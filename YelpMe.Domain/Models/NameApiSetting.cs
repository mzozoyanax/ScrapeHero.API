﻿using System;
using System.Collections.Generic;

namespace YelpMe.Domain.Models;

public class NameApiSetting : BaseEntity
{
    public int SelectApiKeyId { get; set; }

    public bool UseMultipleKeys { get; set; }

    public bool ValidEmail { get; set; }

    public bool HumanNameEmails { get; set; }
}
