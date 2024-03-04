using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        [Authorize]
        [Route("community/{communityId}")]
        [HttpPost]
        public async Task<ActionResult> Add(int communityId)
        {
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			context.Add(new CommunityModerator { AccountId = accountId, CommunityId = communityId, CreatedOn = DateTime.Now });
            context.SaveChanges();
            return Ok();
        }
      
    }
}
