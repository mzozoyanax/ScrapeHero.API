using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YelpMe.Domain.ViewModels
{
    public class SearchViewModels
    {
        [Key]
        public int Id { get; set; }

        public int CloudId { get; set; }

        public string Keywords { get; set; }

        public string Location { get; set; }

        public int Page { get; set; }

        public bool PersonalEmail { get; set; }

        public bool SearchFacebookPixel { get; set; }

        public bool SearchYouTubeChannel { get; set; }

        public bool ContactPage { get; set; }

        public bool AccelerateMode { get; set; }

        public bool SearchOffline { get; set; }
    }
}
