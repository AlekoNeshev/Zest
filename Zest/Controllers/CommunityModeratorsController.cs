using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityModeratorsController : Controller
    {
		private readonly ICommunityModeratorService _communityModeratorService;
	
		public CommunityModeratorsController(ICommunityModeratorService communityModeratorService)
		{
			this._communityModeratorService = communityModeratorService;
			
		}
		[Authorize]
		[Route("isModerator/{accountId}/{communityId}")]
		[HttpGet]
		public async Task<ActionResult<bool>> Find(string accountId, int communityId)
		{
			return await _communityModeratorService.IsModeratorAsync(accountId, communityId);
		}

		[Authorize]
		[Route("add/{accountId}/{communityId}")]
		[HttpPost]
		public async Task<IActionResult> Add(string accountId, int communityId)
		{
			await _communityModeratorService.AddModeratorAsync(accountId, communityId);
			return Ok();
		}

		[Authorize]
		[Route("getModerators/{communityId}")]
		[HttpGet]
		public async Task<UserViewModel[]> GetModeratorsByCommunity(int communityId)
		{
			return await _communityModeratorService.GetModeratorsByCommunityAsync(communityId);
		}

		[Authorize]
		[Route("getCandidates/{communityId}")]
		[HttpGet]
		public async Task<UserViewModel[]> GetModeratorCandidatesByCommunity(int communityId)
		{
			return await _communityModeratorService.GetModeratorCandidatesByCommunityAsync(communityId);
		}

		[Authorize]
		[Route("approveCandidate/{accountId}/{communityId}")]
		[HttpPost]
		public async Task<IActionResult> ApproveCandidate(string accountId, int communityId)
		{
			await _communityModeratorService.ApproveCandidateAsync(accountId, communityId);
			return Ok();
		}

		[Authorize]
		[Route("removeModerator/{accountId}/{communityId}")]
		[HttpPost]
		public async Task<IActionResult> RemoveModerator(string accountId, int communityId)
		{
			await _communityModeratorService.RemoveModeratorAsync(accountId, communityId);
			return Ok();
		}

	}
}
