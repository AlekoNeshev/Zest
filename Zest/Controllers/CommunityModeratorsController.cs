using Microsoft.AspNetCore.Mvc;
using Zest.DBModels;
using Zest.DBModels.Models;

namespace Zest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityModeratorsController : Controller
    {
        private ZestContext context;
        public CommunityModeratorsController(ZestContext context)
        {
            this.context = context;
        }
        public async Task<ActionResult> Add(int accountId, int communityId)
        {
            context.Add(new CommunityModerator { AccountId = accountId, CommunityId = communityId, CreatedOn = DateTime.Now });
            context.SaveChanges();
            return Ok();
        }
        [HttpPost]
        public IActionResult Index()
        {
            return View();
        }
    }
}
