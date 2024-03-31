using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
    [ApiController]
    public class CommunityController : ControllerBase
    {
		private readonly ICommunityService _communityService;
		private readonly ICommunityFollowerService _communityFollowerService;
		private readonly IAccountService _accountService;

		public CommunityController(ICommunityService communityService, ICommunityFollowerService communityFollowerService, IAccountService accountService)
		{
			_communityService = communityService;
			_communityFollowerService = communityFollowerService;
			_accountService=accountService;
		}

		[Route("{id}")]
		[HttpGet]
		public async Task<ActionResult<CommunityViewModel>> Find(int id)
		{
			var accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var community = await _communityService.GetCommunityByIdAsync(id, accountId);
			return community;
		}

		[Route("getAll/{takeCount}/{skipCount}")]
		[HttpGet]
		public async Task<ActionResult<CommunityViewModel[]>> GetAll(int takeCount, int skipCount = 0)
		{
			var accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var communities = await _communityService.GetAllCommunitiesAsync(accountId, skipCount, takeCount);
			foreach (var item in communities)
			{
				item.IsSubscribed = await  _communityFollowerService.DoesExistAsync(accountId, item.Id);
			}
			return communities;
		}

		[Route("add/{name}")]
		[HttpPost]
		public async Task<ActionResult<int>> Add(string name, [FromBody] string discription)
		{
			var creatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			
			var communityId = await _communityService.AddCommunityAsync(creatorId, name, discription);
			return communityId;
		}
		[Route("delete/{communityId}")]
		[HttpDelete]
		public async Task<IActionResult> Delete(int communityId)
		{
			var doesCommunityExists = await _communityService.DoesExistAsync(communityId);
			if (!doesCommunityExists)
			{
				return BadRequest("Community does not exists");
			}
			var creatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			 await _communityService.DeleteCommunityAsync(communityId);
			return Ok();
		}
		[Route("GetByAccountId/{accountId}/{takeCount}/{skipCount}")]
		[HttpGet]
		public async Task<ActionResult<CommunityViewModel[]>> GetCommunitiesByAccount(string accountId, int takeCount, int skipCount = 0)
		{
			var doesAccountExists = await _accountService.DoesExistAsync(accountId);
			if (!doesAccountExists)
			{
				return BadRequest("Account does not exists");
			}
			var communities = await _communityService.GetCommunitiesByAccount(accountId, takeCount, skipCount);
			return communities;
		}
		[Route("GetByPopularityId/{takeCount}")]
		[HttpPost]
		public async Task<ActionResult<CommunityViewModel[]>> GetCommunitiesByPopularity(int takeCount,[FromBody] int[]? skipIds)
		{
			var accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var communities = await _communityService.GetTrendingCommunitiesAsync(skipIds, takeCount, accountId);
			return communities;
		}
		[Route("getBySearch/{search}/{takeCount}")]
		[HttpPost]
		public async Task<ActionResult<CommunityViewModel[]>> GetBySearch(string search, int takeCount, [FromBody] int[]? skipIds)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrWhiteSpace(search))
			{
				return BadRequest("Search is empty!");

			}
			var communities = await _communityService.GetBySearchAsync(search, accountId, takeCount, skipIds);

			return communities;
		}
	}
}
