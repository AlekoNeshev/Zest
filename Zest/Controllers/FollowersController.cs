using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
    [ApiController]
    public class FollowersController : ControllerBase
    {
        private ZestContext context;
		private IMapper mapper;
		public FollowersController(ZestContext context, IMapper mapper)
        {
            this.context=context;
            this.mapper=mapper;
        }
        [Route("followers/find/{followerId}/{followedId}")]
        [HttpGet]
        public async Task<ActionResult<FollowerViewModel>> Find(string followerId, string followedId)
        {
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			return mapper.Map<FollowerViewModel>(context.Followers.Where(x => x.FollowerId == followerId && x.FollowedId == followedId).FirstOrDefault());
        }
        [Route("add/followed/{followedId}")]
        [HttpPost]
        public async Task<ActionResult> Add(string followedId)
        {
			var user = User.Claims;
			var followerId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			context.Add(new Follower { FollowerId = followerId, FollowedId = followedId , CreatedOn = DateTime.Now});
            context.SaveChanges();
            return Ok();
        }
		[Route("delete/followed/{followedId}")]
		[HttpDelete]
		public async Task<ActionResult> Delete(string followedId)
		{
			var user = User.Claims;
			var followerId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var followship = context.Followers.Where(x=>x.FollowerId==followerId && x.FollowedId==followedId).FirstOrDefault();
            context.Remove(followship);
			context.SaveChanges();
			return Ok();
		}
        [Route("getFriends")]
        [HttpGet]
        public async Task<ActionResult<List<FollowerViewModel>>> FindFriends()
        {
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			Follower[] collection = context.Followers.Where(x=>x.FollowedId==accountId).ToArray();
            List<FollowerViewModel> viewModels = new List<FollowerViewModel>();
            foreach (var item in collection)
            {
                if(context.Followers.Where(x=>x.FollowerId == accountId).FirstOrDefault() != null)
                {
                    viewModels.Add(mapper.Map<FollowerViewModel>(item));
                }
            }
            return viewModels;
        }
	}
}
