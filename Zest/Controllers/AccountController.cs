using Microsoft.AspNetCore.Mvc;
using Zest.DBModels;
using Zest.DBModels.Models;

namespace Zest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        public ZestContext zestContext;
        public AccountController(ZestContext zestContext)
        {
            this.zestContext = zestContext;
        }
        [HttpGet]
        public async Task<ActionResult<Account>> Index(int id)
        {
            return zestContext.Accounts.Where(x=>x.Id ==id).FirstOrDefault();
        }
    }
}
