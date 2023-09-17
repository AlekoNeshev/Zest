using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using Zest.DBModels;
using Zest.DBModels.Models;

namespace Zest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityFollowersController : ControllerBase
    {
        private ZestContext context;

        public CommunityFollowersController(ZestContext context)
        {
            this.context = context;
        }
        [Route("account/{accountId}/community/{communityId}")]
        [HttpPost]
        public async Task<ActionResult> Add(int accountId, int communityId)
        {
            context.Add(new CommunityFollower { AccountId = accountId, CommunityId = communityId, CreatedOn = DateTime.Now });
            context.SaveChanges();
            return Ok();
        }
    }
}
