using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YelpMe.Domain.Models;
using YelpMe.Domain.ViewModels;
using YelpMe.Interface.Services;

namespace YelpMe.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchApiController : ControllerBase
    {
        private readonly IScrapeService _scrapeService;

        public SearchApiController(IScrapeService scrapeService)
        {
            _scrapeService = scrapeService;
        }

        [HttpGet]
        public List<Business> GetBusiness()
        {
            return _scrapeService.GetBusiness();
        }

        [HttpPost]
        public async Task SearchBusiness(SearchViewModels searchViewModels)
        {
            await _scrapeService.GetListing(searchViewModels);
        }
    }
}
