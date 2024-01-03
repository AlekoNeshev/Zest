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
		[HttpGet]
		public async Task<ActionResult<bool>> DoesExist(int accountId, int communityId)
		{
			CommunityFollower communityFollower = this.context.CommunityFollowers.Where(x => x.AccountId == accountId && x.CommunityId == communityId).FirstOrDefault();
			if (communityFollower == null)
			{
				return false;
			}

			return true;
		}
		[Route("account/add/{accountId}/community/{communityId}")]
		[HttpPost]
		public async Task<ActionResult> Add(int accountId, int communityId)
		{
			CommunityFollower communityFollower = this.context.CommunityFollowers.Where(x => x.AccountId == accountId && x.CommunityId == communityId).FirstOrDefault();
			if (communityFollower == null)
			{
				context.Add(new CommunityFollower { AccountId = accountId, CommunityId = communityId, CreatedOn = DateTime.Now });
				context.SaveChanges();
			}

			return Ok();
		}
		[Route("account/delete/{accountId}/community/{communityId}")]
		[HttpDelete]
		public async Task<ActionResult> Delete(int accountId, int communityId)
		{
			CommunityFollower communityFollower = this.context.CommunityFollowers.Where(x => x.AccountId == accountId && x.CommunityId == communityId).FirstOrDefault();

			if (communityFollower != null)
			{
				context.Remove(communityFollower);
				context.SaveChanges();
			}
			return Ok();
		}
	}
}
