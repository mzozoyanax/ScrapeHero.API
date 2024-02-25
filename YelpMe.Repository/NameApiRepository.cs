using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YelpMe.Domain.ViewModels;
using YelpMe.Interfaces.Repositories;

namespace YelpMe.Repository
{
    public class NameApiRepository : INameApiRepository
    {
        public async Task<EmailValidationResponse> DisposableEmaiAddressDetector(string email, string apiKey)
        {
            try
            {
                var client = new HttpClient();
                var requestUri = $"https://api.nameapi.org/rest/v5.3/email/disposableemailaddressdetector?apiKey={apiKey}-user1&emailAddress={email}";
                var response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    EmailValidationResponse deserializedResponse = JsonConvert.DeserializeObject<EmailValidationResponse>(jsonResponse);

                    return deserializedResponse;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<NameApiViewModels> EmailNameParser(string email, string apiKey)
        {
            try
            {
                var client = new HttpClient();
                var requestUri = $"https://api.nameapi.org/rest/v5.3/email/emailnameparser?apiKey={apiKey}&emailAddress={email}";
                var response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    NameApiViewModels deserializedResponse = JsonConvert.DeserializeObject<NameApiViewModels>(jsonResponse);

                    return deserializedResponse;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
