using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zest.DBModels;
using Zest.DBModels.Models;

namespace Zest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowersController : ControllerBase
    {
        private ZestContext context;

        public FollowersController(ZestContext context)
        {
            this.context=context;
        }

        [HttpPost]
        public async Task<ActionResult> Add(int followerId, int followedId)
        {
            context.Add(new Follower { FollowerId = followerId, FollowedId = followedId , CreatedOn = DateTime.Now});
            context.SaveChanges();
            return Ok();
        }
    }
}
