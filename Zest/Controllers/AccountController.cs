using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;
namespace Zest.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountService _accountService;
        private IFollowerService _followerService;
        IMapper _mapper;
        public AccountController(IAccountService accountService, IFollowerService followerService,IMapper mapper)
        {
            this._accountService = accountService;
            this._followerService = followerService;
            this._mapper = mapper;
        }
 
        [Route("get")]
        [HttpGet]
        public async Task<ActionResult<AccountViewModel>> FindById()
        {
            var accountId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var account = await _accountService.FindByIdAsync(accountId);
            var accountViewModel = _mapper.Map<AccountViewModel>(account);
            return Ok(accountViewModel);
        }
       
        [Route("add/{name}/{email}")]
        [HttpPost]
        public async Task<ActionResult<AccountViewModel>> Add(string name, string email)
        {
            var accountId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var account = _accountService.AddAsync(accountId, name, email);
            var newAccount = _mapper.Map<AccountViewModel>(account);
       
            return Ok(newAccount);
        }
		[Route("getAll")]
		[HttpGet]
		public async Task<ActionResult<UserViewModel[]>> GetAll()
		{
			
			var accountId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var accounts = await _accountService.GetAllAsync(accountId);
            var userViewModels = _mapper.Map<UserViewModel[]>(accounts);
			foreach (var item in userViewModels)
			{
                
                
				item.IsFollowed = await _followerService.FindAsync(accountId, item.Id) != null;
			}
			return Ok(userViewModels);
		}
	}
}
