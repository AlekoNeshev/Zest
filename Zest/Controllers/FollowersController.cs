using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
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
        public async Task<ActionResult<FollowerViewModel>> Find(int followerId, int followedId)
        {
            return mapper.Map<FollowerViewModel>(context.Followers.Where(x => x.FollowerId == followerId && x.FollowedId == followedId).FirstOrDefault());
        }
        [Route("add/{followerId}/followed/{followedId}")]
        [HttpPost]
        public async Task<ActionResult> Add(int followerId, int followedId)
        {
            context.Add(new Follower { FollowerId = followerId, FollowedId = followedId , CreatedOn = DateTime.Now});
            context.SaveChanges();
            return Ok();
        }
		[Route("delete/{followerId}/followed/{followedId}")]
		[HttpDelete]
		public async Task<ActionResult> Delete(int followerId, int followedId)
		{
			var followship = context.Followers.Where(x=>x.FollowerId==followerId && x.FollowedId==followedId).FirstOrDefault();
            context.Remove(followship);
			context.SaveChanges();
			return Ok();
		}
        [Route("account/{accountId}")]
        [HttpGet]
        public async Task<ActionResult<List<FollowerViewModel>>> FindFriends(int accountId)
        {
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
