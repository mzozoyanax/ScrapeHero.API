using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YelpMe.Domain;
using YelpMe.Domain.Models;
using YelpMe.Interface.Services;
using YelpMe.Interfaces.Services;

namespace YelpMe.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountApiController : ControllerBase
    {
        private readonly IEntityService<Account> _entityService;
        private readonly AppDbContext _appDbContext;

        public AccountApiController(IEntityService<Account> entityService, AppDbContext appDbContext)
        {
            _entityService = entityService;
            _appDbContext = appDbContext;
        }

        [HttpGet(nameof(GetAccountById))]
        public IActionResult GetAccountById(int Id)
        {
            var obj = _entityService.Get(Id);
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(obj);
            }
        }
        [HttpGet(nameof(GetAllAccounts))]
        public IActionResult GetAllAccounts()
        {
            var obj = _entityService.GetAll();
            if (obj == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(obj);
            }
        }
        [HttpPost(nameof(CreateAccount))]
        public IActionResult CreateAccount(Account entity)
        {
            if (entity != null)
            {
                _entityService.Add(entity);
                return Ok("Created Successfully");
            }
            else
            {
                return BadRequest("Something went wrong");
            }
        }
        [HttpPost(nameof(UpdateAccount))]
        public IActionResult UpdateAccount(Account entity)
        {
            if (entity != null)
            {
                _entityService.Update(entity);
                return Ok("Updated SuccessFully");
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpDelete(nameof(DeleteAccount))]
        public IActionResult DeleteAccount(Account entity)
        {
            if (entity != null)
            {
                _entityService.Delete(entity);
                return Ok("Deleted Successfully");
            }
            else
            {
                return BadRequest("Something went wrong");
            }
        }
    }
}
