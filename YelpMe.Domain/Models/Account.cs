using System;
using System.Collections.Generic;

namespace YelpMe.Domain.Models;

public class Account 
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string SmtpHost { get; set; } = null!;

    public int SmtpPort { get; set; }

    public string ImapHost { get; set; } = null!;

    public int ImapPort { get; set; }

    public bool StmpSsl { get; set; }

    public bool ImapSsl { get; set; }

    public string? Signature { get; set; }
}
