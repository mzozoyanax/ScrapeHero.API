using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YelpMe.Domain.ViewModels;

namespace YelpMe.Interfaces.Repositories
{
    public interface INameApiRepository
    {
        Task<NameApiViewModels> EmailNameParser(string email, string apiKey);

        Task<EmailValidationResponse> DisposableEmaiAddressDetector(string email, string apiKey);
    }
}
