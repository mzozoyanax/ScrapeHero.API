using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YelpMe.Interface.Repositories;
using YelpMe.Interface.Services;

namespace YelpMe.Services
{
    public class ScrapeService : IScrapeService
    {
        private readonly IScrapeRepository _scrapeRepository;

        public ScrapeService(IScrapeRepository scrapeRepository)
        {
            _scrapeRepository = scrapeRepository;  
        }

        public Task<bool> ContainsFacebookPixelCode(YelpMe.Domain.ViewModels.SearchViewModels searchViewModels, string websiteUrl)
        {
            return _scrapeRepository.ContainsFacebookPixelCode(searchViewModels, websiteUrl);
        }

        public Task<bool> ContainYouTubeChannel(YelpMe.Domain.ViewModels.SearchViewModels searchViewModels, string websiteUrl)
        {
            return _scrapeRepository.ContainYouTubeChannel(searchViewModels, websiteUrl);
        }

        public Task<string> ConvertWebsiteToHtml(string url)
        {
            return _scrapeRepository.ConvertWebsiteToHtml(url);
        }

        public Task<string> ConvertWebsiteToText(string url)
        {
            return _scrapeRepository.ConvertWebsiteToText(url);
        }

        public bool DeleteBusiness(YelpMe.Domain.Models.Business business)
        {
            return _scrapeRepository.DeleteBusiness(business);
        }

        public Task<string> FindBusinessWebsite(string profileUrl, bool contactPage)
        {
            return _scrapeRepository.FindBusinessWebsite(profileUrl, contactPage);
        }

        public Task<string> FindEmailAddress(string profileUrl, bool contactPage)
        {
            return _scrapeRepository.FindEmailAddress(profileUrl, contactPage);
        }

        public List<YelpMe.Domain.Models.Business> GetBusiness()
        {
            return _scrapeRepository.GetBusiness();
        }

        public Task<string> GetBusinessWebsite(string profileUrl)
        {
            return _scrapeRepository.GetFacebookPage(profileUrl);
        }

        public Task<string> GetCompanyName(string profileUrl)
        {
            return _scrapeRepository.GetCompanyName(profileUrl);
        }

        public Task<string> GetEmailAddress(string websiteUrl)
        {
            return _scrapeRepository.GetEmailAddress(websiteUrl);   
        }

        public Task<string> GetEmailAddressFromContactPage(string websiteUrl)
        {
            return _scrapeRepository.GetEmailAddressFromContactPage(websiteUrl);
        }

        public Task<string> GetFacebookPage(string websiteUrl)
        {
            return _scrapeRepository.GetFacebookPage(websiteUrl);
        }

        public Task<string> GetInstagram(string websiteUrl)
        {
            return _scrapeRepository.GetInstagram(websiteUrl);
        }

        public Task<string> GetLinkedIn(string websiteUrl)
        {
            return _scrapeRepository.GetLinkedIn(websiteUrl);
        }

        public Task<bool> GetListing(YelpMe.Domain.ViewModels.SearchViewModels searchViewModels)
        {
            return _scrapeRepository.GetListing(searchViewModels);
        }

        public Task<string> GetOwnersName(string profileUrl)
        {
            return _scrapeRepository.GetOwnersName(profileUrl);
        }

        public Task<string> GetPhoneNumber(string profileUrl)
        {
            return _scrapeRepository.GetPhoneNumber(profileUrl);
        }

        public Task<string> GetYouTubeChannel(string websiteUrl)
        {
            return _scrapeRepository.GetYouTubeChannel(websiteUrl);
        }

        public bool UpdateBusiness(YelpMe.Domain.Models.Business business)
        {
            return _scrapeRepository.UpdateBusiness(business);
        }

        public Task<bool> ValidUrl(string url)
        {
            return _scrapeRepository.ValidUrl(url);
        }
    }
}
