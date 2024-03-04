using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using System.Security.Claims;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
	[Authorize]
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
 
        [Route("get")]
        [HttpGet]
        public async Task<ActionResult<AccountViewModel>> FindById()
        {
            var id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

			return mapper.Map<AccountViewModel>(zestContext.Accounts.Where(x => x.Id ==id).FirstOrDefault());
        }
        /* [Route("email/{email}/password/{password}")]
         [HttpGet]
         public async Task<ActionResult<AccountViewModel>> FindByEmail(string email, string password)
         {
             return mapper.Map<AccountViewModel>(zestContext.Accounts.Where(x=>x.Email ==email && x.Password==password).FirstOrDefault());
         }*/
        [Route("add/{name}/{email}")]
        [HttpPost]
        public async Task<ActionResult> Add(string name, string email)
        {
            var user = User.Claims;
            var p = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
           
			var newAccount = zestContext.Add(new Account
            {          
                Id = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
			    Username = name,
			    Email = email,
                CreatedOn = DateTime.Now,
			    IsAdmin = false

            }) ;
            zestContext.SaveChanges();
			var accountId = newAccount.Property<string>("Id").CurrentValue;
            var username = newAccount.Property<string>("Username").CurrentValue;
			return Ok(new string []{ accountId, username});
        }
		[Route("getAll")]
		[HttpGet]
		public async Task<ActionResult<UserViewModel[]>> GetAll()
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			UserViewModel[] userViewModels = mapper.Map<UserViewModel[]>(zestContext.Accounts.ToArray());
			foreach (var item in userViewModels)
			{
				item.IsFollowed = zestContext.Followers.Where(x => x.FollowerId == accountId && x.FollowedId == item.Id).FirstOrDefault() != null;
			}
            return userViewModels;
		}
	}
}
