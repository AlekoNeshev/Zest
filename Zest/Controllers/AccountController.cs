using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zest.Services.Infrastructure.Interfaces;
using Zest.Services.Infrastructure.Services;
using Zest.ViewModels.ViewModels;
namespace Zest.Controllers
{
	[Authorize]
	[Route("Zest/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountService _accountService;
        
        public AccountController(IAccountService accountService)
        {
            this._accountService = accountService;      
        }
 
        [Route("get")]
        [HttpGet]
        public async Task<ActionResult<AccountViewModel>> FindById()
        {
            var accountId = User.Id();
			var account = await _accountService.FindByIdAsync(accountId);

            return account;
        }
       
        [Route("add/{name}/{email}")]
        [HttpPost]
        public async Task<ActionResult<AccountViewModel>> Add(string name, string email)
        {
            var accountId = User.Id();
			var doesUsernameExist = _accountService.FindByUsernameAsync(name);
            if(doesUsernameExist == null)
            {
                return BadRequest("Username already exists!");
            }
            var account = await _accountService.AddAsync(accountId, name, email);
   
            return account;
        }
		[Route("getAll/{takeCount}/{skipCount}")]
		[HttpGet]
		public async Task<ActionResult<UserViewModel[]>> GetAll(int takeCount, int skipCount = 0)
		{

            var accountId = User.Id();
			var accounts = await _accountService.GetAllAsync(accountId, takeCount, skipCount);
           			
			return accounts;
		}
		[Route("getBySearch/{search}/{takeCount}")]
		[HttpPost]
		public async Task<ActionResult<UserViewModel[]>> GetBySearch(string search, int takeCount, [FromBody] string[]? skipIds)
		{
            if (string.IsNullOrWhiteSpace(search))
            {
                return BadRequest("Search is empty!");

            }
			var accountId = User.Id();
			var accounts = await _accountService.GetBySearchAsync(search, accountId, takeCount, skipIds);

			return accounts;
		}
	}
}
