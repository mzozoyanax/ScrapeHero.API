using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using YelpMe.Domain;
using YelpMe.Domain.Constants;
using YelpMe.Domain.Models;
using YelpMe.Domain.ViewModels;
using YelpMe.Interface.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace YelpMe.Repository
{
    public class ScrapeRepository : IScrapeRepository
    {
        private AppDbContext appDbContext = new AppDbContext();
        private Setting setting = new Setting();
        private NameApiSetting nameApiSettings = new NameApiSetting();
        private NameApiRepository nameAPIRepo = new NameApiRepository();
        private AppConstant appConstant = new AppConstant();

        public ScrapeRepository() 
        {
            setting = appDbContext.Settings.FirstOrDefault();
            nameApiSettings = appDbContext.NameApiSettings.FirstOrDefault();
        }   
        public async Task<string> ConvertWebsiteToHtml(string url)
        {
            try
            {
                // Create an instance of HttpClient
                using (HttpClient client = new HttpClient())
                {
                    // Send a GET request to the website
                    HttpResponseMessage response = await client.GetAsync(url);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the HTML content as a string
                        string htmlString = await response.Content.ReadAsStringAsync();

                        return htmlString;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> ConvertWebsiteToText(string url)
        {
            try
            {
                // Create an instance of HttpClient
                using (HttpClient client = new HttpClient())
                {
                    // Send a GET request to the website
                    HttpResponseMessage response = await client.GetAsync(url);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the HTML content as a string
                        string htmlString = await response.Content.ReadAsStringAsync();

                        const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
                        const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
                        const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
                        var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
                        var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
                        var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

                        var text = htmlString;
                        //Decode html specific characters
                        text = System.Net.WebUtility.HtmlDecode(text);
                        //Remove tag whitespace/line breaks
                        text = tagWhiteSpaceRegex.Replace(text, "><");
                        //Replace <br /> with line breaks
                        text = lineBreakRegex.Replace(text, Environment.NewLine);
                        //Strip formatting
                        text = stripFormattingRegex.Replace(text, string.Empty);

                        return text;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> GetBusinessWebsite(string profileUrl)
        {
            string linkUrl = "";

            try
            {
                string htmlContent = await ConvertWebsiteToHtml(profileUrl);

                // Load the HTML document using HtmlAgilityPack
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlContent);

                // "//a[contains(@class, 'css-1idmmu3')]"
                var anchorNodes = doc.DocumentNode.SelectNodes("//a[contains(@class, 'website-link dockable')]");

                if (anchorNodes != null)
                {
                    string url = anchorNodes[0].Attributes["href"].Value;

                    bool isWebsite = await ValidUrl(url);

                    // Extract the link URL and inner text of the <a> tag within the <p> tag
                    if (isWebsite == true)
                    {
                        linkUrl = url;

                        return linkUrl;
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }

            return linkUrl;
        }

        public async Task<string> GetCompanyName(string profileUrl)
        {
            try
            {
                string htmlContent = await ConvertWebsiteToHtml(profileUrl);

                // Load the HTML document using HtmlAgilityPack
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlContent);

                //h1[contains(@class, 'css-1se8maq')]
                var anchorNode = doc.DocumentNode.SelectSingleNode("//h1[contains(@class, 'dockable business-name')]");

                if (anchorNode != null)
                {
                    // Extract the inner text of the <a> tag
                    string name = anchorNode.InnerText;

                    return name;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public async Task<string> GetEmailAddress(string websiteUrl)
        {
            string email = "";

            try
            {
                string htmlContent = await ConvertWebsiteToHtml(websiteUrl);

                // Load the HTML document using HtmlAgilityPack
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlContent);

                //a
                var anchorNodes = doc.DocumentNode.SelectNodes("//a");

                if (anchorNodes != null)
                {
                    foreach (var anchorNode in anchorNodes)
                    {
                        string pageLink = anchorNode.Attributes["href"].Value;
                        string input = await ConvertWebsiteToHtml(pageLink);

                        // Define a regular expression pattern to match valid email addresses
                        string pattern = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b";

                        // Create a regular expression object and match the pattern in the input string
                        Regex regex = new Regex(pattern);
                        MatchCollection matches = regex.Matches(input);

                        // Report on each match.
                        foreach (Match match in matches)
                        {
                            bool validEmail = match.Value.Contains(".com");

                            if (validEmail == true)
                            {
                                email = match.Value;

                                return email;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }

            return email;
        }

        public async Task<string> GetOwnersName(string profileUrl)
        {
            try
            {
                string htmlContent = await ConvertWebsiteToHtml(profileUrl);

                // Load the HTML document using HtmlAgilityPack
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlContent);

                //h1[contains(@class, 'css-1se8maq')]
                var anchorNode = doc.DocumentNode.SelectSingleNode("//dd[contains(@class, 'general-info')]");

                if (anchorNode != null)
                {
                    // Extract the inner text of the <a> tag
                    string name = anchorNode.InnerText.Substring(0, 15);

                    return name;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public async Task<string> GetPhoneNumber(string profileUrl)
        {
            string phoneNumber = "";

            try
            {
                string htmlContent = await ConvertWebsiteToHtml(profileUrl);

                // Load the HTML document
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlContent);

                // Select the anchor tag with the specified class
                var anchorNode = doc.DocumentNode.SelectSingleNode("//a[@class='phone dockable']");

                if (anchorNode != null)
                {
                    // Get the href attribute value
                    string hrefValue = anchorNode.GetAttributeValue("href", "");

                    phoneNumber = "1" + hrefValue.Replace("tel:", "")
                        .Replace("(", "")
                        .Replace(")", "")
                        .Replace("-", "")
                        .Replace(" ", "");

                }
            }
            catch (Exception)
            {
                return "";
            }

            return phoneNumber;
        }

        public async Task<bool> ValidUrl(string url)
        {
            string pattern = @"^(http|https|ftp)://([\w-]+(\.[\w-]+)+([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?)$";
            return Regex.IsMatch(url, pattern);
        }

        public async Task<bool> ContainsFacebookPixelCode(SearchViewModels searchViewModels, string websiteUrl)
        {
            bool result = false;

            try
            {
                WebClient client = new WebClient();
                string pageSource = client.DownloadString(websiteUrl);

                if (pageSource.Contains("https://connect.facebook.net/en_US/fbevents.js"))
                {
                    result = true;
                }
                else
                {
                    result = false;
                }

                if (setting.FacebookPixelInstalled == result)
                {
                    result = setting.FacebookPixelInstalled;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return result;
        }

        public async Task<bool> ContainYouTubeChannel(SearchViewModels searchViewModels, string websiteUrl)
        {
            bool result = false;

            try
            {
                string htmlContent = await ConvertWebsiteToHtml(websiteUrl);

                // Load the HTML document using HtmlAgilityPack
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlContent);

                // Select all anchor tags <a> with href attributes containing "youtube.com/channel/"
                var aNodesWithYouTubeLink = doc.DocumentNode.SelectNodes("//a[contains(@href, 'youtube.com/')]");

                if (aNodesWithYouTubeLink != null && aNodesWithYouTubeLink.Count > 0)
                {
                    result = true;
                    // If at least one YouTube channel link is found, return true
                }
                else
                {
                    result = false;
                    // If no YouTube channel link is found, return false
                }

                if (setting.HasYouTubeChannel == result)
                {
                    result = setting.HasYouTubeChannel;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception)
            {
                // Handle exceptions gracefully and return false in case of any errors
                return false;
            }

            return result;
        }

        public async Task<string> GetFacebookPage(string websiteUrl)
        {
            string result = "";

            try
            {
                string htmlContent = await ConvertWebsiteToHtml(websiteUrl);

                // Load the HTML document using HtmlAgilityPack
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlContent);

                //a
                var anchorNodes = doc.DocumentNode.SelectNodes("//a");

                if (anchorNodes != null)
                {
                    foreach (var anchorNode in anchorNodes)
                    {
                        string pageLink = anchorNode.Attributes["href"].Value;

                        var web = new HtmlWeb();
                        var doc2 = web.Load(pageLink);

                        string xpathExpression = "//a[contains(@href, 'facebook.com')]/@href";
                        HtmlNode facebookLinkNode = doc.DocumentNode.SelectSingleNode(xpathExpression);

                        if (facebookLinkNode != null)
                        {
                            result = facebookLinkNode.GetAttributeValue("href", "");

                            return result;
                        }
                        else
                        {
                            return "";
                        }
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }

            return result;
        }

        public async Task<string> GetYouTubeChannel(string websiteUrl)
        {
            string result = "";

            try
            {
                string htmlContent = await ConvertWebsiteToHtml(websiteUrl);

                // Load the HTML document using HtmlAgilityPack
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlContent);

                //a
                var anchorNodes = doc.DocumentNode.SelectNodes("//a");

                if (anchorNodes != null)
                {
                    foreach (var anchorNode in anchorNodes)
                    {
                        string pageLink = anchorNode.Attributes["href"].Value;

                        var web = new HtmlWeb();
                        var doc2 = web.Load(pageLink);

                        string xpathExpression = "//a[contains(@href, 'youtube.com')]/@href";
                        HtmlNode facebookLinkNode = doc.DocumentNode.SelectSingleNode(xpathExpression);

                        if (facebookLinkNode != null)
                        {
                            result = facebookLinkNode.GetAttributeValue("href", "");

                            return result;
                        }
                        else
                        {
                            return "";
                        }
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }

            return result;
        }

        public async Task<string> GetEmailAddressFromContactPage(string websiteUrl)
        {
            string email = "";

            try
            {
                string baseUrl = await ConvertWebsiteToText(websiteUrl);
                string htmlContent = baseUrl + "/contact";

                // Define a regular expression pattern to match valid email addresses
                string pattern = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b";

                // Create a regular expression object and match the pattern in the input string
                Regex regex = new Regex(pattern);
                MatchCollection matches = regex.Matches(htmlContent);

                // Report on each match.
                foreach (Match match in matches)
                {
                    bool validEmail = match.Value.Contains(".com");

                    if (validEmail == true)
                    {
                        email = match.Value.Split(".com")[0];
                        email = email + ".com";

                        return email;
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }

            return email;
        }

        public async Task<string> FindEmailAddress(string profileUrl, bool contactPage)
        {
            if (contactPage)
            {
                return await GetEmailAddressFromContactPage(profileUrl);
            }
            else
            {
                return await GetEmailAddress(profileUrl);
            }
        }

        public async Task<string> FindBusinessWebsite(string profileUrl, bool contactPage)
        {
            if (contactPage)
            {
                var baseUrl = await GetBusinessWebsite(profileUrl);
                var url = baseUrl.Split(".com")[0] + ".com/contact";

                if (baseUrl == null)
                {
                    return "";
                }
                else
                {
                    return url;
                }

            }
            else
            {
                return await GetBusinessWebsite(profileUrl);
            }
        }

        public async Task<bool> GetListing(SearchViewModels searchViewModel)
        {
            try
            {
                //Check the current page...
                int index = 0;

                string baseURL = "https://www.yellowpages.com";
                string keywords = searchViewModel.Keywords;
                string location = searchViewModel.Location;
                bool facebookPixel = searchViewModel.SearchFacebookPixel;
                bool youTubeChannel = searchViewModel.SearchYouTubeChannel;
                bool accerelateModel = searchViewModel.AccelerateMode;
                bool noFilter = (searchViewModel.SearchFacebookPixel == false || searchViewModel.SearchYouTubeChannel == false);
                int page = searchViewModel.Page;
                bool personalEmail = searchViewModel.PersonalEmail;

                bool useContactPage = searchViewModel.ContactPage;

                int nameApiKeyRowIndex = 0;

                for (var i = 0; i < page; i++)
                {
                    try
                    {
                        string url = $"{baseURL}/search?search_terms={keywords.Replace(" ", "+")}&geo_location_terms={location.Replace(" ", "+")}&page={index}";

                        string htmlContent = await ConvertWebsiteToHtml(url);

                        // Load the HTML document using HtmlAgilityPack
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(htmlContent);

                        // Select anchor nodes
                        var anchorNodes = doc.DocumentNode.SelectNodes("//a[contains(@class, 'business-name')]");

                        if (anchorNodes != null)
                        {
                            foreach (var anchorNode in anchorNodes)
                            {
                                if (anchorNode.Attributes["href"].Value.Contains("/mip"))
                                {
                                    string profileUrl = baseURL + anchorNode.Attributes["href"].Value;
                                    string websiteUrl = await FindBusinessWebsite(profileUrl, useContactPage);

                                    string email = await FindEmailAddress(websiteUrl, useContactPage);

                                    if (accerelateModel == true)
                                    {
                                        var query = appDbContext.Business.Where(x => x.Email == email).FirstOrDefault();

                                        if (query == null)
                                        {
                                            Business business = new Business();

                                            business.Keywords = keywords;
                                            business.Location = location;
                                            business.Name = await GetCompanyName(profileUrl);
                                            business.Email = email;
                                            business.Phone = await GetPhoneNumber(profileUrl);
                                            business.Profile = profileUrl;
                                            business.Website = websiteUrl;
                                            business.FacebookPage = await GetFacebookPage(websiteUrl);
                                            business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                            business.Instagram = await GetInstagram(websiteUrl);
                                            business.LinkedIn = await GetLinkedIn(websiteUrl);
                                            business.Company = await GetCompanyName(profileUrl);
                                            business.PersonalLine = "";
                                            business.Sent = false;

                                            if (facebookPixel == true)
                                            {
                                                business.FacebookPage = await GetFacebookPage(websiteUrl);
                                            }

                                            if (youTubeChannel == true)
                                            {
                                                business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                            }

                                            appDbContext.Business.Add(business);
                                            appDbContext.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        if (email != "")
                                        {
                                            var query = appDbContext.Business.Where(x => x.Email == email).FirstOrDefault();

                                            if (query == null)
                                            {
                                                if (personalEmail == true)
                                                {
                                                    bool isPersonalEmail = appConstant.PersonalEmail(email);

                                                    if (isPersonalEmail)
                                                    {
                                                        if (setting.UseNameApi == true)
                                                        {
                                                            /*
                                                            Disabled: no
                                                            Credits/Month: 10000
                                                            Credits/Day: 1000
                                                            Credits/Hour: 300
                                                            Credits/Minute: 30
                                                            Simultaneous: 1
                                                            Min Response Time: 500ms
                                                            Interval: 2023-10-19
                                                            Credits spent: 12
                                                            Total requests: 7
                                                            Successful requests: 6
                                                            */

                                                            //Get the properties for getting information about the email validation...
                                                            EmailValidationResponse validEmail = new EmailValidationResponse();

                                                            //Get the properties for getting information about the persons name that is using the email...
                                                            NameApiViewModels emailRealName = new NameApiViewModels();

                                                            //If you want to use multiple api to increase your NAME API procedures without request limitation, use this...
                                                            if (nameApiSettings.UseMultipleKeys == true)
                                                            {
                                                                //Get all the NAME API Keys that you saved into your database...
                                                                List<NameApiKey> apiKeys = appDbContext.NameApiKeys.ToList();

                                                                //Check these this option are enabled so you don't the API request carelessily...
                                                                bool enableEmailValidate = nameApiSettings.ValidEmail;
                                                                bool enableEmailRealName = nameApiSettings.HumanNameEmails;

                                                                //Just the prospects name holder here, once you have found their real name using NAME API.
                                                                string userName = null;

                                                                //The NAME API Key that the machine has selected to use to verify this current lead...
                                                                string selectedApiKey = apiKeys[nameApiKeyRowIndex].ApiKey;

                                                                //If this option is true, you are free to use the NAME API requests
                                                                if (noFilter == true)
                                                                {
                                                                    if (enableEmailValidate == true)
                                                                    {
                                                                        validEmail = await nameAPIRepo.DisposableEmaiAddressDetector(email, selectedApiKey);

                                                                        if (validEmail.disposable == "NO")
                                                                        {
                                                                            if (enableEmailRealName == true)
                                                                            {
                                                                                emailRealName = await nameAPIRepo.EmailNameParser(email, selectedApiKey);

                                                                                if (emailRealName.ResultType == "PERSON_NAME")
                                                                                {
                                                                                    userName = emailRealName.NameMatches[0].GivenNames[0].Name;

                                                                                    Business business = new Business();

                                                                                    business.Keywords = keywords;
                                                                                    business.Location = location;
                                                                                    business.Name = await GetCompanyName(profileUrl);
                                                                                    business.Email = email;
                                                                                    business.Phone = await GetPhoneNumber(profileUrl);
                                                                                    business.Profile = profileUrl;
                                                                                    business.Website = websiteUrl;
                                                                                    business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                                    business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                                    business.Instagram = await GetInstagram(websiteUrl);
                                                                                    business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                                    business.Company = await GetCompanyName(profileUrl);
                                                                                    business.PersonalLine = "";
                                                                                    business.Sent = false;

                                                                                    appDbContext.Business.Add(business);
                                                                                    appDbContext.SaveChanges();
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                Business business = new Business();

                                                                                business.Keywords = keywords;
                                                                                business.Location = location;
                                                                                business.Name = await GetCompanyName(profileUrl);
                                                                                business.Email = email;
                                                                                business.Phone = await GetPhoneNumber(profileUrl);
                                                                                business.Profile = profileUrl;
                                                                                business.Website = websiteUrl;
                                                                                business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                                business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                                business.Instagram = await GetInstagram(websiteUrl);
                                                                                business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                                business.Company = await GetCompanyName(profileUrl);
                                                                                business.PersonalLine = "";
                                                                                business.Sent = false;

                                                                                appDbContext.Business.Add(business);
                                                                                appDbContext.SaveChanges();
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (enableEmailRealName == true)
                                                                        {
                                                                            emailRealName = await nameAPIRepo.EmailNameParser(email, selectedApiKey);

                                                                            if (emailRealName.ResultType == "PERSON_NAME")
                                                                            {
                                                                                userName = emailRealName.NameMatches[0].GivenNames[0].Name;

                                                                                Business business = new Business();

                                                                                business.Keywords = keywords;
                                                                                business.Location = location;
                                                                                business.Name = await GetCompanyName(profileUrl);
                                                                                business.Email = email;
                                                                                business.Phone = await GetPhoneNumber(profileUrl);
                                                                                business.Profile = profileUrl;
                                                                                business.Website = websiteUrl;
                                                                                business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                                business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                                business.Instagram = await GetInstagram(websiteUrl);
                                                                                business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                                business.Company = await GetCompanyName(profileUrl);
                                                                                business.PersonalLine = "";
                                                                                business.Sent = false;

                                                                                appDbContext.Business.Add(business);
                                                                                appDbContext.SaveChanges();
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    bool hasFacebookPixel = await ContainsFacebookPixelCode(searchViewModel, websiteUrl);
                                                                    bool hasYouTubeChannel = await ContainYouTubeChannel(searchViewModel, websiteUrl);

                                                                    if (hasFacebookPixel == true || hasYouTubeChannel == true)
                                                                    {
                                                                        if (enableEmailValidate == true)
                                                                        {
                                                                            validEmail = await nameAPIRepo.DisposableEmaiAddressDetector(email, selectedApiKey);

                                                                            if (validEmail.disposable == "NO")
                                                                            {
                                                                                if (enableEmailRealName == true)
                                                                                {
                                                                                    emailRealName = await nameAPIRepo.EmailNameParser(email, selectedApiKey);

                                                                                    if (emailRealName.ResultType == "PERSON_NAME")
                                                                                    {
                                                                                        userName = emailRealName.NameMatches[0].GivenNames[0].Name;

                                                                                        Business business = new Business();

                                                                                        business.Keywords = keywords;
                                                                                        business.Location = location;
                                                                                        business.Name = await GetCompanyName(profileUrl);
                                                                                        business.Email = email;
                                                                                        business.Phone = await GetPhoneNumber(profileUrl);
                                                                                        business.Profile = profileUrl;
                                                                                        business.Website = websiteUrl;
                                                                                        business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                                        business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                                        business.Instagram = await GetInstagram(websiteUrl);
                                                                                        business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                                        business.Company = await GetCompanyName(profileUrl);
                                                                                        business.PersonalLine = "";
                                                                                        business.Sent = false;

                                                                                        appDbContext.Business.Add(business);
                                                                                        appDbContext.SaveChanges();
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    Business business = new Business();

                                                                                    business.Keywords = keywords;
                                                                                    business.Location = location;
                                                                                    business.Name = await GetCompanyName(profileUrl);
                                                                                    business.Email = email;
                                                                                    business.Phone = await GetPhoneNumber(profileUrl);
                                                                                    business.Profile = profileUrl;
                                                                                    business.Website = websiteUrl;
                                                                                    business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                                    business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                                    business.Instagram = await GetInstagram(websiteUrl);
                                                                                    business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                                    business.Company = await GetCompanyName(profileUrl);
                                                                                    business.PersonalLine = "";
                                                                                    business.Sent = false;

                                                                                    appDbContext.Business.Add(business);
                                                                                    appDbContext.SaveChanges();
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (enableEmailRealName == true)
                                                                            {
                                                                                emailRealName = await nameAPIRepo.EmailNameParser(email, selectedApiKey);

                                                                                if (emailRealName.ResultType == "PERSON_NAME")
                                                                                {
                                                                                    userName = emailRealName.NameMatches[0].GivenNames[0].Name;

                                                                                    Business business = new Business();

                                                                                    business.Keywords = keywords;
                                                                                    business.Location = location;
                                                                                    business.Name = await GetCompanyName(profileUrl);
                                                                                    business.Email = email;
                                                                                    business.Phone = await GetPhoneNumber(profileUrl);
                                                                                    business.Profile = profileUrl;
                                                                                    business.Website = websiteUrl;
                                                                                    business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                                    business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                                    business.Instagram = await GetInstagram(websiteUrl);
                                                                                    business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                                    business.Company = await GetCompanyName(profileUrl);
                                                                                    business.PersonalLine = "";
                                                                                    business.Sent = false;

                                                                                    appDbContext.Business.Add(business);
                                                                                    appDbContext.SaveChanges();
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }

                                                                nameApiKeyRowIndex = (nameApiKeyRowIndex + 1) % apiKeys.Count;
                                                            }
                                                            else
                                                            {
                                                                //Get all the NAME API Keys that you saved into your database...
                                                                List<NameApiKey> apiKey = appDbContext.NameApiKeys.ToList();

                                                                //Check these this option are enabled so you don't the API request carelessily...
                                                                bool enableEmailValidate = nameApiSettings.ValidEmail;
                                                                bool enableEmailRealName = nameApiSettings.HumanNameEmails;

                                                                //Just the prospects name holder here, once you have found their real name using NAME API.
                                                                string userName = null;

                                                                //The NAME API Key that the machine has selected to use to verify this current lead...
                                                                string selectedApiKey = apiKey.Where(x => x.Id == nameApiSettings.SelectApiKeyId).FirstOrDefault().ApiKey;

                                                                //If this option is true, you are free to use the NAME API requests

                                                                if (noFilter == true)
                                                                {
                                                                    if (enableEmailValidate == true)
                                                                    {
                                                                        validEmail = await nameAPIRepo.DisposableEmaiAddressDetector(email, selectedApiKey);

                                                                        if (validEmail.disposable == "NO")
                                                                        {
                                                                            if (enableEmailRealName == true)
                                                                            {
                                                                                emailRealName = await nameAPIRepo.EmailNameParser(email, selectedApiKey);

                                                                                if (emailRealName.ResultType == "PERSON_NAME")
                                                                                {
                                                                                    userName = emailRealName.NameMatches[0].GivenNames[0].Name;

                                                                                    Business business = new Business();

                                                                                    business.Keywords = keywords;
                                                                                    business.Location = location;
                                                                                    business.Name = await GetCompanyName(profileUrl);
                                                                                    business.Email = email;
                                                                                    business.Phone = await GetPhoneNumber(profileUrl);
                                                                                    business.Profile = profileUrl;
                                                                                    business.Website = websiteUrl;
                                                                                    business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                                    business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                                    business.Instagram = await GetInstagram(websiteUrl);
                                                                                    business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                                    business.Company = await GetCompanyName(profileUrl);
                                                                                    business.PersonalLine = "";
                                                                                    business.Sent = false;

                                                                                    appDbContext.Business.Add(business);
                                                                                    appDbContext.SaveChanges();
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                Business business = new Business();

                                                                                business.Keywords = keywords;
                                                                                business.Location = location;
                                                                                business.Name = await GetCompanyName(profileUrl);
                                                                                business.Email = email;
                                                                                business.Phone = await GetPhoneNumber(profileUrl);
                                                                                business.Profile = profileUrl;
                                                                                business.Website = websiteUrl;
                                                                                business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                                business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                                business.Instagram = await GetInstagram(websiteUrl);
                                                                                business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                                business.Company = await GetCompanyName(profileUrl);
                                                                                business.PersonalLine = "";
                                                                                business.Sent = false;

                                                                                appDbContext.Business.Add(business);
                                                                                appDbContext.SaveChanges();
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (enableEmailRealName == true)
                                                                        {
                                                                            emailRealName = await nameAPIRepo.EmailNameParser(email, selectedApiKey);

                                                                            if (emailRealName.ResultType == "PERSON_NAME")
                                                                            {
                                                                                userName = emailRealName.NameMatches[0].GivenNames[0].Name;

                                                                                Business business = new Business();

                                                                                business.Keywords = keywords;
                                                                                business.Location = location;
                                                                                business.Name = await GetCompanyName(profileUrl);
                                                                                business.Email = email;
                                                                                business.Phone = await GetPhoneNumber(profileUrl);
                                                                                business.Profile = profileUrl;
                                                                                business.Website = websiteUrl;
                                                                                business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                                business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                                business.Instagram = await GetInstagram(websiteUrl);
                                                                                business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                                business.Company = await GetCompanyName(profileUrl);
                                                                                business.PersonalLine = "";
                                                                                business.Sent = false;

                                                                                appDbContext.Business.Add(business);
                                                                                appDbContext.SaveChanges();
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    bool hasFacebookPixel = await ContainsFacebookPixelCode(searchViewModel, websiteUrl);
                                                                    bool hasYouTubeChannel = await ContainYouTubeChannel(searchViewModel, websiteUrl);

                                                                    if (hasFacebookPixel == true || hasYouTubeChannel == true)
                                                                    {
                                                                        if (enableEmailValidate == true)
                                                                        {
                                                                            validEmail = await nameAPIRepo.DisposableEmaiAddressDetector(email, selectedApiKey);

                                                                            if (validEmail.disposable == "NO")
                                                                            {
                                                                                if (enableEmailRealName == true)
                                                                                {
                                                                                    emailRealName = await nameAPIRepo.EmailNameParser(email, selectedApiKey);

                                                                                    if (emailRealName.ResultType == "PERSON_NAME")
                                                                                    {
                                                                                        userName = emailRealName.NameMatches[0].GivenNames[0].Name;

                                                                                        Business business = new Business();

                                                                                        business.Keywords = keywords;
                                                                                        business.Location = location;
                                                                                        business.Name = await GetCompanyName(profileUrl);
                                                                                        business.Email = email;
                                                                                        business.Phone = await GetPhoneNumber(profileUrl);
                                                                                        business.Profile = profileUrl;
                                                                                        business.Website = websiteUrl;
                                                                                        business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                                        business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                                        business.Instagram = await GetInstagram(websiteUrl);
                                                                                        business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                                        business.Company = await GetCompanyName(profileUrl);
                                                                                        business.PersonalLine = "";
                                                                                        business.Sent = false;

                                                                                        appDbContext.Business.Add(business);
                                                                                        appDbContext.SaveChanges();
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    userName = emailRealName.NameMatches[0].GivenNames[0].Name;

                                                                                    Business business = new Business();

                                                                                    business.Keywords = keywords;
                                                                                    business.Location = location;
                                                                                    business.Name = await GetCompanyName(profileUrl);
                                                                                    business.Email = email;
                                                                                    business.Phone = await GetPhoneNumber(profileUrl);
                                                                                    business.Profile = profileUrl;
                                                                                    business.Website = websiteUrl;
                                                                                    business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                                    business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                                    business.Instagram = await GetInstagram(websiteUrl);
                                                                                    business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                                    business.Company = await GetCompanyName(profileUrl);
                                                                                    business.PersonalLine = "";
                                                                                    business.Sent = false;

                                                                                    appDbContext.Business.Add(business);
                                                                                    appDbContext.SaveChanges();
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (enableEmailRealName == true)
                                                                            {
                                                                                emailRealName = await nameAPIRepo.EmailNameParser(email, selectedApiKey);

                                                                                if (emailRealName.ResultType == "PERSON_NAME")
                                                                                {
                                                                                    userName = emailRealName.NameMatches[0].GivenNames[0].Name;

                                                                                    Business business = new Business();

                                                                                    business.Keywords = keywords;
                                                                                    business.Location = location;
                                                                                    business.Name = await GetCompanyName(profileUrl);
                                                                                    business.Email = email;
                                                                                    business.Phone = await GetPhoneNumber(profileUrl);
                                                                                    business.Profile = profileUrl;
                                                                                    business.Website = websiteUrl;
                                                                                    business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                                    business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                                    business.Instagram = await GetInstagram(websiteUrl);
                                                                                    business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                                    business.Company = await GetCompanyName(profileUrl);
                                                                                    business.PersonalLine = "";
                                                                                    business.Sent = false;
                                                                                    

                                                                                    appDbContext.Business.Add(business);
                                                                                    appDbContext.SaveChanges();
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (noFilter == true)
                                                            {
                                                                Business business = new Business();

                                                                business.Keywords = keywords;
                                                                business.Location = location;
                                                                business.Name = await GetCompanyName(profileUrl);
                                                                business.Email = email;
                                                                business.Phone = await GetPhoneNumber(profileUrl);
                                                                business.Profile = profileUrl;
                                                                business.Website = websiteUrl;
                                                                business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                business.Instagram = await GetInstagram(websiteUrl);
                                                                business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                business.Company = await GetCompanyName(profileUrl);
                                                                business.PersonalLine = "";
                                                                business.Sent = false;
                                                                

                                                                if (facebookPixel == true)
                                                                {
                                                                    business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                }

                                                                if (youTubeChannel == true)
                                                                {
                                                                    business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                }

                                                                appDbContext.Business.Add(business);
                                                                appDbContext.SaveChanges();

                                                            }
                                                            else
                                                            {
                                                                bool hasFacebookPixel = await ContainsFacebookPixelCode(searchViewModel, websiteUrl);
                                                                bool hasYouTubeChannel = await ContainYouTubeChannel(searchViewModel, websiteUrl);

                                                                if (hasFacebookPixel == true || hasYouTubeChannel == true)
                                                                {
                                                                    Business business = new Business();

                                                                    business.Keywords = keywords;
                                                                    business.Location = location;
                                                                    business.Name = await GetCompanyName(profileUrl);
                                                                    business.Email = email;
                                                                    business.Phone = await GetPhoneNumber(profileUrl);
                                                                    business.Profile = profileUrl;
                                                                    business.Website = websiteUrl;
                                                                    business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                    business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                    business.Instagram = await GetInstagram(websiteUrl);
                                                                    business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                    business.Company = await GetCompanyName(profileUrl);
                                                                    business.PersonalLine = "";
                                                                    business.Sent = false;
                                                                    

                                                                    appDbContext.Business.Add(business);
                                                                    appDbContext.SaveChanges();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (setting.UseNameApi == true)
                                                    {
                                                        /*
                                                        Disabled: no
                                                        Credits/Month: 10000
                                                        Credits/Day: 1000
                                                        Credits/Hour: 300
                                                        Credits/Minute: 30
                                                        Simultaneous: 1
                                                        Min Response Time: 500ms
                                                        Interval: 2023-10-19
                                                        Credits spent: 12
                                                        Total requests: 7
                                                        Successful requests: 6
                                                        */

                                                        //Get the properties for getting information about the email validation...
                                                        EmailValidationResponse validEmail = new EmailValidationResponse();

                                                        //Get the properties for getting information about the persons name that is using the email...
                                                        NameApiViewModels emailRealName = new NameApiViewModels();

                                                        //If you want to use multiple api to increase your NAME API procedures without request limitation, use this...
                                                        if (nameApiSettings.UseMultipleKeys == true)
                                                        {
                                                            //Get all the NAME API Keys that you saved into your database...
                                                            List<NameApiKey> apiKeys = appDbContext.NameApiKeys.ToList();

                                                            //Check these this option are enabled so you don't the API request carelessily...
                                                            bool enableEmailValidate = nameApiSettings.ValidEmail;
                                                            bool enableEmailRealName = nameApiSettings.HumanNameEmails;

                                                            //Just the prospects name holder here, once you have found their real name using NAME API.
                                                            string userName = null;

                                                            //The NAME API Key that the machine has selected to use to verify this current lead...
                                                            string selectedApiKey = apiKeys[nameApiKeyRowIndex].ApiKey;

                                                            //If this option is true, you are free to use the NAME API requests
                                                            if (noFilter == true)
                                                            {
                                                                if (enableEmailValidate == true)
                                                                {
                                                                    validEmail = await nameAPIRepo.DisposableEmaiAddressDetector(email, selectedApiKey);

                                                                    if (validEmail.disposable == "NO")
                                                                    {
                                                                        if (enableEmailRealName == true)
                                                                        {
                                                                            emailRealName = await nameAPIRepo.EmailNameParser(email, selectedApiKey);

                                                                            if (emailRealName.ResultType == "PERSON_NAME")
                                                                            {
                                                                                userName = emailRealName.NameMatches[0].GivenNames[0].Name;

                                                                                Business business = new Business();

                                                                                business.Keywords = keywords;
                                                                                business.Location = location;
                                                                                business.Name = await GetCompanyName(profileUrl);
                                                                                business.Email = email;
                                                                                business.Phone = await GetPhoneNumber(profileUrl);
                                                                                business.Profile = profileUrl;
                                                                                business.Website = websiteUrl;
                                                                                business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                                business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                                business.Instagram = await GetInstagram(websiteUrl);
                                                                                business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                                business.Company = await GetCompanyName(profileUrl);
                                                                                business.PersonalLine = "";
                                                                                business.Sent = false;
                                                                                

                                                                                appDbContext.Business.Add(business);
                                                                                appDbContext.SaveChanges();
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            Business business = new Business();

                                                                            business.Keywords = keywords;
                                                                            business.Location = location;
                                                                            business.Name = await GetCompanyName(profileUrl);
                                                                            business.Email = email;
                                                                            business.Phone = await GetPhoneNumber(profileUrl);
                                                                            business.Profile = profileUrl;
                                                                            business.Website = websiteUrl;
                                                                            business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                            business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                            business.Instagram = await GetInstagram(websiteUrl);
                                                                            business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                            business.Company = await GetCompanyName(profileUrl);
                                                                            business.PersonalLine = "";
                                                                            business.Sent = false;

                                                                            appDbContext.Business.Add(business);
                                                                            appDbContext.SaveChanges();
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (enableEmailRealName == true)
                                                                    {
                                                                        emailRealName = await nameAPIRepo.EmailNameParser(email, selectedApiKey);

                                                                        if (emailRealName.ResultType == "PERSON_NAME")
                                                                        {
                                                                            userName = emailRealName.NameMatches[0].GivenNames[0].Name;

                                                                            Business business = new Business();

                                                                            business.Keywords = keywords;
                                                                            business.Location = location;
                                                                            business.Name = await GetCompanyName(profileUrl);
                                                                            business.Email = email;
                                                                            business.Phone = await GetPhoneNumber(profileUrl);
                                                                            business.Profile = profileUrl;
                                                                            business.Website = websiteUrl;
                                                                            business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                            business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                            business.Instagram = await GetInstagram(websiteUrl);
                                                                            business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                            business.Company = await GetCompanyName(profileUrl);
                                                                            business.PersonalLine = "";
                                                                            business.Sent = false;
                                                                            

                                                                            appDbContext.Business.Add(business);
                                                                            appDbContext.SaveChanges();
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                bool hasFacebookPixel = await ContainsFacebookPixelCode(searchViewModel, websiteUrl);
                                                                bool hasYouTubeChannel = await ContainYouTubeChannel(searchViewModel, websiteUrl);

                                                                if (hasFacebookPixel == true || hasYouTubeChannel == true)
                                                                {
                                                                    if (enableEmailValidate == true)
                                                                    {
                                                                        validEmail = await nameAPIRepo.DisposableEmaiAddressDetector(email, selectedApiKey);

                                                                        if (validEmail.disposable == "NO")
                                                                        {
                                                                            if (enableEmailRealName == true)
                                                                            {
                                                                                emailRealName = await nameAPIRepo.EmailNameParser(email, selectedApiKey);

                                                                                if (emailRealName.ResultType == "PERSON_NAME")
                                                                                {
                                                                                    userName = emailRealName.NameMatches[0].GivenNames[0].Name;

                                                                                    Business business = new Business();

                                                                                    business.Keywords = keywords;
                                                                                    business.Location = location;
                                                                                    business.Name = await GetCompanyName(profileUrl);
                                                                                    business.Email = email;
                                                                                    business.Phone = await GetPhoneNumber(profileUrl);
                                                                                    business.Profile = profileUrl;
                                                                                    business.Website = websiteUrl;
                                                                                    business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                                    business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                                    business.Instagram = await GetInstagram(websiteUrl);
                                                                                    business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                                    business.Company = await GetCompanyName(profileUrl);
                                                                                    business.PersonalLine = "";
                                                                                    business.Sent = false;
                                                                                    

                                                                                    appDbContext.Business.Add(business);
                                                                                    appDbContext.SaveChanges();
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                Business business = new Business();

                                                                                business.Keywords = keywords;
                                                                                business.Location = location;
                                                                                business.Name = await GetCompanyName(profileUrl);
                                                                                business.Email = email;
                                                                                business.Phone = await GetPhoneNumber(profileUrl);
                                                                                business.Profile = profileUrl;
                                                                                business.Website = websiteUrl;
                                                                                business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                                business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                                business.Instagram = await GetInstagram(websiteUrl);
                                                                                business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                                business.Company = await GetCompanyName(profileUrl);
                                                                                business.PersonalLine = "";
                                                                                business.Sent = false;

                                                                                appDbContext.Business.Add(business);
                                                                                appDbContext.SaveChanges();
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (enableEmailRealName == true)
                                                                        {
                                                                            emailRealName = await nameAPIRepo.EmailNameParser(email, selectedApiKey);

                                                                            if (emailRealName.ResultType == "PERSON_NAME")
                                                                            {
                                                                                userName = emailRealName.NameMatches[0].GivenNames[0].Name;

                                                                                Business business = new Business();

                                                                                business.Keywords = keywords;
                                                                                business.Location = location;
                                                                                business.Name = await GetCompanyName(profileUrl);
                                                                                business.Email = email;
                                                                                business.Phone = await GetPhoneNumber(profileUrl);
                                                                                business.Profile = profileUrl;
                                                                                business.Website = websiteUrl;
                                                                                business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                                business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                                business.Instagram = await GetInstagram(websiteUrl);
                                                                                business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                                business.Company = await GetCompanyName(profileUrl);
                                                                                business.PersonalLine = "";
                                                                                business.Sent = false;
                                                                                

                                                                                appDbContext.Business.Add(business);
                                                                                appDbContext.SaveChanges();
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            nameApiKeyRowIndex = (nameApiKeyRowIndex + 1) % apiKeys.Count;
                                                        }
                                                        else
                                                        {
                                                            //Get all the NAME API Keys that you saved into your database...
                                                            List<NameApiKey> apiKey = appDbContext.NameApiKeys.ToList();

                                                            //Check these this option are enabled so you don't the API request carelessily...
                                                            bool enableEmailValidate = nameApiSettings.ValidEmail;
                                                            bool enableEmailRealName = nameApiSettings.HumanNameEmails;

                                                            //Just the prospects name holder here, once you have found their real name using NAME API.
                                                            string userName = null;

                                                            //The NAME API Key that the machine has selected to use to verify this current lead...
                                                            string selectedApiKey = apiKey.Where(x => x.Id == nameApiSettings.SelectApiKeyId).FirstOrDefault().ApiKey;

                                                            //If this option is true, you are free to use the NAME API requests

                                                            if (noFilter == true)
                                                            {
                                                                if (enableEmailValidate == true)
                                                                {
                                                                    validEmail = await nameAPIRepo.DisposableEmaiAddressDetector(email, selectedApiKey);

                                                                    if (validEmail.disposable == "NO")
                                                                    {
                                                                        if (enableEmailRealName == true)
                                                                        {
                                                                            emailRealName = await nameAPIRepo.EmailNameParser(email, selectedApiKey);

                                                                            if (emailRealName.ResultType == "PERSON_NAME")
                                                                            {
                                                                                userName = emailRealName.NameMatches[0].GivenNames[0].Name;

                                                                                Business business = new Business();

                                                                                business.Keywords = keywords;
                                                                                business.Location = location;
                                                                                business.Name = await GetCompanyName(profileUrl);
                                                                                business.Email = email;
                                                                                business.Phone = await GetPhoneNumber(profileUrl);
                                                                                business.Profile = profileUrl;
                                                                                business.Website = websiteUrl;
                                                                                business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                                business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                                business.Instagram = await GetInstagram(websiteUrl);
                                                                                business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                                business.Company = await GetCompanyName(profileUrl);
                                                                                business.PersonalLine = "";
                                                                                business.Sent = false;
                                                                                

                                                                                appDbContext.Business.Add(business);
                                                                                appDbContext.SaveChanges();
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            Business business = new Business();

                                                                            business.Keywords = keywords;
                                                                            business.Location = location;
                                                                            business.Name = await GetCompanyName(profileUrl);
                                                                            business.Email = email;
                                                                            business.Phone = await GetPhoneNumber(profileUrl);
                                                                            business.Profile = profileUrl;
                                                                            business.Website = websiteUrl;
                                                                            business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                            business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                            business.Instagram = await GetInstagram(websiteUrl);
                                                                            business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                            business.Company = await GetCompanyName(profileUrl);
                                                                            business.PersonalLine = "";
                                                                            business.Sent = false;
                                                                            

                                                                            appDbContext.Business.Add(business);
                                                                            appDbContext.SaveChanges();
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (enableEmailRealName == true)
                                                                    {
                                                                        emailRealName = await nameAPIRepo.EmailNameParser(email, selectedApiKey);

                                                                        if (emailRealName.ResultType == "PERSON_NAME")
                                                                        {
                                                                            userName = emailRealName.NameMatches[0].GivenNames[0].Name;

                                                                            Business business = new Business();

                                                                            business.Keywords = keywords;
                                                                            business.Location = location;
                                                                            business.Name = await GetCompanyName(profileUrl);
                                                                            business.Email = email;
                                                                            business.Phone = await GetPhoneNumber(profileUrl);
                                                                            business.Profile = profileUrl;
                                                                            business.Website = websiteUrl;
                                                                            business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                            business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                            business.Instagram = await GetInstagram(websiteUrl);
                                                                            business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                            business.Company = await GetCompanyName(profileUrl);
                                                                            business.PersonalLine = "";
                                                                            business.Sent = false;
                                                                            

                                                                            appDbContext.Business.Add(business);
                                                                            appDbContext.SaveChanges();
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                bool hasFacebookPixel = await ContainsFacebookPixelCode(searchViewModel, websiteUrl);
                                                                bool hasYouTubeChannel = await ContainYouTubeChannel(searchViewModel, websiteUrl);

                                                                if (hasFacebookPixel == true || hasYouTubeChannel == true)
                                                                {
                                                                    if (enableEmailValidate == true)
                                                                    {
                                                                        validEmail = await nameAPIRepo.DisposableEmaiAddressDetector(email, selectedApiKey);

                                                                        if (validEmail.disposable == "NO")
                                                                        {
                                                                            if (enableEmailRealName == true)
                                                                            {
                                                                                emailRealName = await nameAPIRepo.EmailNameParser(email, selectedApiKey);

                                                                                if (emailRealName.ResultType == "PERSON_NAME")
                                                                                {
                                                                                    userName = emailRealName.NameMatches[0].GivenNames[0].Name;

                                                                                    Business business = new Business();

                                                                                    business.Keywords = keywords;
                                                                                    business.Location = location;
                                                                                    business.Name = await GetCompanyName(profileUrl);
                                                                                    business.Email = email;
                                                                                    business.Phone = await GetPhoneNumber(profileUrl);
                                                                                    business.Profile = profileUrl;
                                                                                    business.Website = websiteUrl;
                                                                                    business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                                    business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                                    business.Instagram = await GetInstagram(websiteUrl);
                                                                                    business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                                    business.Company = await GetCompanyName(profileUrl);
                                                                                    business.PersonalLine = "";
                                                                                    business.Sent = false;
                                                                                    

                                                                                    appDbContext.Business.Add(business);
                                                                                    appDbContext.SaveChanges();
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                userName = emailRealName.NameMatches[0].GivenNames[0].Name;

                                                                                Business business = new Business();

                                                                                business.Keywords = keywords;
                                                                                business.Location = location;
                                                                                business.Name = await GetCompanyName(profileUrl);
                                                                                business.Email = email;
                                                                                business.Phone = await GetPhoneNumber(profileUrl);
                                                                                business.Profile = profileUrl;
                                                                                business.Website = websiteUrl;
                                                                                business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                                business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                                business.Instagram = await GetInstagram(websiteUrl);
                                                                                business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                                business.Company = await GetCompanyName(profileUrl);
                                                                                business.PersonalLine = "";
                                                                                business.Sent = false;
                                                                                

                                                                                appDbContext.Business.Add(business);
                                                                                appDbContext.SaveChanges();
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (enableEmailRealName == true)
                                                                        {
                                                                            emailRealName = await nameAPIRepo.EmailNameParser(email, selectedApiKey);

                                                                            if (emailRealName.ResultType == "PERSON_NAME")
                                                                            {
                                                                                userName = emailRealName.NameMatches[0].GivenNames[0].Name;

                                                                                Business business = new Business();

                                                                                business.Keywords = keywords;
                                                                                business.Location = location;
                                                                                business.Name = await GetCompanyName(profileUrl);
                                                                                business.Email = email;
                                                                                business.Phone = await GetPhoneNumber(profileUrl);
                                                                                business.Profile = profileUrl;
                                                                                business.Website = websiteUrl;
                                                                                business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                                business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                                business.Instagram = await GetInstagram(websiteUrl);
                                                                                business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                                business.Company = await GetCompanyName(profileUrl);
                                                                                business.PersonalLine = "";
                                                                                business.Sent = false;
                                                                                

                                                                                appDbContext.Business.Add(business);
                                                                                appDbContext.SaveChanges();
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (noFilter == true)
                                                        {
                                                            Business business = new Business();

                                                            business.Keywords = keywords;
                                                            business.Location = location;
                                                            business.Name = await GetCompanyName(profileUrl);
                                                            business.Email = email;
                                                            business.Phone = await GetPhoneNumber(profileUrl);
                                                            business.Profile = profileUrl;
                                                            business.Website = websiteUrl;
                                                            business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                            business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                            business.Instagram = await GetInstagram(websiteUrl);
                                                            business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                            business.Company = await GetCompanyName(profileUrl);
                                                            business.PersonalLine = "";
                                                            business.Sent = false;

                                                            appDbContext.Business.Add(business);
                                                            appDbContext.SaveChanges();

                                                        }
                                                        else
                                                        {
                                                            bool hasFacebookPixel = await ContainsFacebookPixelCode(searchViewModel, websiteUrl);
                                                            bool hasYouTubeChannel = await ContainYouTubeChannel(searchViewModel, websiteUrl);

                                                            if (hasFacebookPixel == true || hasYouTubeChannel == true)
                                                            {
                                                                Business business = new Business();

                                                                business.Keywords = keywords;
                                                                business.Location = location;
                                                                business.Name = await GetCompanyName(profileUrl);
                                                                business.Email = email;
                                                                business.Phone = await GetPhoneNumber(profileUrl);
                                                                business.Profile = profileUrl;
                                                                business.Website = websiteUrl;
                                                                business.FacebookPage = await GetFacebookPage(websiteUrl);
                                                                business.YouTubeChannel = await GetYouTubeChannel(websiteUrl);
                                                                business.Instagram = await GetInstagram(websiteUrl);
                                                                business.LinkedIn = await GetLinkedIn(websiteUrl);
                                                                business.Company = await GetCompanyName(profileUrl);
                                                                business.PersonalLine = "";
                                                                business.Sent = false;

                                                                appDbContext.Business.Add(business);
                                                                appDbContext.SaveChanges();
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            return false;
                        }

                        index += 10;

                    }
                    catch (Exception ex)
                    {
                    }
                }

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public List<Business> GetBusiness()
        {
            return appDbContext.Business.ToList();
        }

        public bool UpdateBusiness(Business business)
        {
            try
            {
                var query = appDbContext.Business.Where(x => x.Id == business.Id).FirstOrDefault();

                query.Keywords = business.Website;
                query.Location = business.Location;
                query.Name = business.Location;
                query.Email = business.Location;
                query.Phone = business.Location;
                query.Profile = business.Location;
                query.Website = business.Location;
                query.FacebookPage = business.Location;
                query.YouTubeChannel = business.Location;
                query.Company = business.Location;
                query.PersonalLine = business.Location;
                query.Sent = business.Sent;

                appDbContext.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool DeleteBusiness(Business business)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetInstagram(string websiteUrl)
        {
            string result = "";

            try
            {
                string htmlContent = await ConvertWebsiteToHtml(websiteUrl);

                // Load the HTML document using HtmlAgilityPack
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlContent);

                //a
                var anchorNodes = doc.DocumentNode.SelectNodes("//a");

                if (anchorNodes != null)
                {
                    foreach (var anchorNode in anchorNodes)
                    {
                        string pageLink = anchorNode.Attributes["href"].Value;

                        var web = new HtmlWeb();
                        var doc2 = web.Load(pageLink);

                        string xpathExpression = "//a[contains(@href, 'instagram.com')]/@href";
                        HtmlNode facebookLinkNode = doc.DocumentNode.SelectSingleNode(xpathExpression);

                        if (facebookLinkNode != null)
                        {
                            result = facebookLinkNode.GetAttributeValue("href", "");

                            return result;
                        }
                        else
                        {
                            return "";
                        }
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }

            return result;
        }

        public async Task<string> GetLinkedIn(string websiteUrl)
        {
            string result = "";

            try
            {
                string htmlContent = await ConvertWebsiteToHtml(websiteUrl);

                // Load the HTML document using HtmlAgilityPack
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlContent);

                //a
                var anchorNodes = doc.DocumentNode.SelectNodes("//a");

                if (anchorNodes != null)
                {
                    foreach (var anchorNode in anchorNodes)
                    {
                        string pageLink = anchorNode.Attributes["href"].Value;

                        var web = new HtmlWeb();
                        var doc2 = web.Load(pageLink);

                        string xpathExpression = "//a[contains(@href, 'linkedin.com.com')]/@href";
                        HtmlNode facebookLinkNode = doc.DocumentNode.SelectSingleNode(xpathExpression);

                        if (facebookLinkNode != null)
                        {
                            result = facebookLinkNode.GetAttributeValue("href", "");

                            return result;
                        }
                        else
                        {
                            return "";
                        }
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }

            return result;
        }
    }
}
