using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YelpMe.Domain.Constants
{
    public class AppConstant
    {
        public bool PersonalEmail(string email)
        {
            bool result = (
                !email.Contains("no-reply@") ||
                !email.Contains("admin@") ||
                !email.Contains("support@") ||
                !email.Contains("sales@") ||
                !email.Contains("info@") ||
                !email.Contains("contact@") ||
                !email.Contains("abuse@") ||
                !email.Contains("spam@") ||
                !email.Contains("webmaster@") ||
                !email.Contains("privacy@") ||
                !email.Contains("postmaster@") ||
                !email.Contains("unsubscribe@") ||
                !email.Contains("root@") ||
                !email.Contains("marketing@") ||
                !email.Contains("noreply@") ||
                !email.Contains("test@") ||
                !email.Contains("contactus@") ||
                !email.Contains("help@") ||
                !email.Contains("ceo@") ||
                !email.Contains("contact@")
            );

            return result;
        }
    }
}
