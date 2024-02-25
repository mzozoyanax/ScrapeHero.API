using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YelpMe.Domain.Models;
using YelpMe.Domain.ViewModels;

namespace YelpMe.Interface.Services
{
    public interface IScrapeService
    {
        List<Business> GetBusiness();

        bool UpdateBusiness(Business business);

        bool DeleteBusiness(Business business);

        Task<bool> GetListing(SearchViewModels searchViewModels);

        Task<string> GetOwnersName(string profileUrl);

        Task<string> FindEmailAddress(string profileUrl, bool contactPage);

        Task<string> FindBusinessWebsite(string profileUrl, bool contactPage);

        Task<string> GetEmailAddress(string websiteUrl);

        Task<string> GetEmailAddressFromContactPage(string websiteUrl);

        Task<string> GetPhoneNumber(string profileUrl);

        Task<string> GetBusinessWebsite(string profileUrl);

        Task<string> GetCompanyName(string profileUrl);

        Task<string> ConvertWebsiteToHtml(string url);

        Task<string> ConvertWebsiteToText(string url);

        Task<string> GetFacebookPage(string websiteUrl);

        Task<string> GetYouTubeChannel(string websiteUrl);

        Task<string> GetInstagram(string websiteUrl);

        Task<string> GetLinkedIn(string websiteUrl);

        Task<bool> ContainsFacebookPixelCode(SearchViewModels searchViewModels, string websiteUrl);

        Task<bool> ContainYouTubeChannel(SearchViewModels searchViewModels, string websiteUrl);

        Task<bool> ValidUrl(string url);
    }
}
