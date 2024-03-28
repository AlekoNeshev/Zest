using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Services.Infrastructure.Interfaces;
using Zest.Services.Infrastructure.Services;
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
		

		public CommunityController(ICommunityService communityService, ICommunityFollowerService communityFollowerService)
		{
			_communityService = communityService;
			_communityFollowerService = communityFollowerService;
			
		}

		[Route("{id}")]
		[HttpGet]
		public async Task<ActionResult<CommunityViewModel>> Find(int id)
		{
			var community = await _communityService.GetCommunityByIdAsync(id);
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
		public async Task<ActionResult> Add(string name, [FromBody] string discription)
		{
			var creatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			
			var communityId = await _communityService.AddCommunityAsync(creatorId, name, discription);
			return Ok(communityId);
		}
		[Route("GetByAccountId/{accountId}/{takeCount}/{skipCount}")]
		[HttpGet]
		public async Task<ActionResult<CommunityViewModel[]>> GetCommunitiesByAccount(string accountId, int takeCount, int skipCount = 0)
		{
			var communities = await _communityService.GetCommunitiesByAccount(accountId, takeCount, skipCount);
			return communities;
		}
		[Route("GetByPopularityId/{takeCount}")]
		[HttpPost]
		public async Task<ActionResult<CommunityViewModel[]>> GetCommunitiesByPopularity(int takeCount,[FromBody] int[]? skipIds)
		{
			var communities = await _communityService.GetTrendingCommunities(skipIds, takeCount);
			return communities;
		}
		[Route("getBySearch/{search}/{takeCount}")]
		[HttpPost]
		public async Task<ActionResult<CommunityViewModel[]>> GetBySearch(string search, int takeCount, [FromBody] int[]? skipIds)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var communities = await _communityService.GetBySearchAsync(search, takeCount, skipIds);

			return communities;
		}
	}
}
