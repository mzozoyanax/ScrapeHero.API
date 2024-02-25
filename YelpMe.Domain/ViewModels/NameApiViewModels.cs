using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YelpMe.Domain.ViewModels
{
    public class GivenName
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("nameType")]
        public string NameType { get; set; }
    }

    public class Surname
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("nameType")]
        public string NameType { get; set; }
    }

    public class NameMatch
    {
        [JsonProperty("givenNames")]
        public List<GivenName> GivenNames { get; set; }

        [JsonProperty("surnames")]
        public List<Surname> Surnames { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }
    }

    public class EmailValidationResponse
    {
        public string disposable { get; set; }
    }

    public class NameApiViewModels
    {
        [JsonProperty("resultType")]
        public string ResultType { get; set; }

        [JsonProperty("nameMatches")]
        public List<NameMatch> NameMatches { get; set; }

        [JsonProperty("bestNameMatch")]
        public NameMatch BestNameMatch { get; set; }
    }
}
