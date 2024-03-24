using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;
using Zest.DBModels;
using Zest.DBModels.Models;
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

		[Route("getAll")]
		[HttpGet]
		public async Task<ActionResult<CommunityViewModel[]>> GetAll()
		{
			var accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var communities = await _communityService.GetAllCommunitiesAsync(accountId);
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
		[Route("GetByAccountId/{accountId}")]
		[HttpGet]
		public async Task<ActionResult<CommunityViewModel[]>> GetCommunitiesByAccount(string accountId)
		{
			var communities = await _communityService.GetCommunitiesByAccount(accountId);
			return communities;
		}
		[Route("GetByPopularityId/{takeCount}")]
		[HttpPost]
		public async Task<ActionResult<CommunityViewModel[]>> GetCommunitiesByPopularity(int takeCount,[FromBody] int[]? skipIds)
		{
			var communities = await _communityService.GetTrendingCommunities(skipIds, takeCount);
			return communities;
		}
	}
}
