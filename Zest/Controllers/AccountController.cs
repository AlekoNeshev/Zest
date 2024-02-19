using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private ZestContext zestContext;
        private IMapper mapper;
        public AccountController(ZestContext zestContext, IMapper mapper)
        {
            this.zestContext = zestContext;
            this.mapper = mapper;
        }
        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult<AccountViewModel>> Index(int id)
        {
            return mapper.Map<AccountViewModel>(zestContext.Accounts.Where(x => x.Id ==id).FirstOrDefault());
        }
        [Route("email/{email}/password/{password}")]
        [HttpGet]
        public async Task<ActionResult<AccountViewModel>> FindByEmail(string email, string password)
        {
            return mapper.Map<AccountViewModel>(zestContext.Accounts.Where(x=>x.Email ==email && x.Password==password).FirstOrDefault());
        }
        [Route("add")]
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] NewAccountViewModel account)
        {
            var newAccount = zestContext.Add(new Account
            {
                FirstName = account.FirstName,
                LastName = account.LastName,
                Username = account.Username,
                Email = account.Email,
                Password = account.Password,
                Birthdate = account.Birthdate,
                CreatedOn = account.CreatedOn1,
                IsAdmin = false

            }) ;
            zestContext.SaveChanges();
			var accountId = newAccount.Property<int>("Id").CurrentValue;
			return Ok(accountId);
        }
		[Route("getAll/{accountId}")]
		[HttpGet]
		public async Task<ActionResult<UserViewModel[]>> GetAll(int accountId)
		{
			UserViewModel[] userViewModels = mapper.Map<UserViewModel[]>(zestContext.Accounts.ToArray());
			foreach (var item in userViewModels)
			{
				item.IsFollowed = zestContext.Followers.Where(x => x.FollowerId == accountId && x.FollowedId == item.Id).FirstOrDefault() != null;
			}
            return userViewModels;
		}
	}
}
