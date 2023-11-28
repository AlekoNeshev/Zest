using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
        [Route("add/{firstName}/{lastName}/{email}/{birthdate}")]
        [HttpPost]
        public async Task<ActionResult> Add(string firstName, string lastName, string username, string email,[FromBody] string password, DateTime birthdate)
        {
            zestContext.Add(new Account
            {
                FirstName = firstName,
                LastName = lastName,
                Username = username,
                Email = email,
                Password = password,
                Birthdate = birthdate,
                CreatedOn = DateTime.Now,
                IsAdmin = false

            }) ;
            zestContext.SaveChanges();
            return Ok();
        }

    }
}
