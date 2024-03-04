using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Zest.DBModels;
using Zest.DBModels.Models;

namespace Zest.Controllers
{
	[Authorize]
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
		public async Task<ActionResult<bool>> DoesExist(int communityId)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			CommunityFollower communityFollower = this.context.CommunityFollowers.Where(x => x.AccountId == accountId && x.CommunityId == communityId).FirstOrDefault();
			if (communityFollower == null)
			{
				return false;
			}

			return true;
		}
		
		[Route("account/add/community/{communityId}")]
		[HttpPost]
		public async Task<ActionResult> Add(int communityId)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			CommunityFollower communityFollower = this.context.CommunityFollowers.Where(x => x.AccountId == accountId && x.CommunityId == communityId).FirstOrDefault();
			if (communityFollower == null)
			{
				context.Add(new CommunityFollower { AccountId = accountId, CommunityId = communityId, CreatedOn = DateTime.Now });
				context.SaveChanges();
			}

			return Ok();
		}
		
		[Route("account/delete/community/{communityId}")]
		[HttpDelete]
		public async Task<ActionResult> Delete(int communityId)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
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
