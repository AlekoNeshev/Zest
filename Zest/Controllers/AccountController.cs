using Microsoft.AspNetCore.Mvc;
using Zest.DBModels;
using Zest.DBModels.Models;

namespace Zest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private ZestContext zestContext;
        public AccountController(ZestContext zestContext)
        {
            this.zestContext = zestContext;
        }
        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult<Account>> Index(int id)
        {
            return zestContext.Accounts.Where(x=>x.Id ==id).FirstOrDefault();
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
