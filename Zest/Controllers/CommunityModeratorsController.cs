using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
	[Route("api/[controller]")]
    [ApiController]
    public class CommunityModeratorsController : Controller
    {
		private readonly ICommunityModeratorService _communityModeratorService;
	private readonly ICommunityService _communityService;
		private readonly IAccountService _accountService;
		public CommunityModeratorsController(ICommunityModeratorService communityModeratorService, ICommunityService communityService, IAccountService accountService)
		{
			this._communityModeratorService = communityModeratorService;
			_communityService=communityService;
			_accountService=accountService;
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
			var isCandidate = await _communityModeratorService.IsModeratorCandidateAsync(accountId, communityId);
			if (isCandidate)
			{
				return BadRequest("User is already candidate");
			}
			await _communityModeratorService.AddModeratorAsync(accountId, communityId);
			return Ok();
		}

		[Authorize]
		[Route("getModerators/{communityId}")]
		[HttpGet]
		public async Task<ActionResult<UserViewModel[]>> GetModeratorsByCommunity(int communityId)
		{
			var doesCommunityExists = await _communityService.DoesExistAsync(communityId);
			if (!doesCommunityExists)
			{
				return BadRequest("Community does not exists");
			}
			return await _communityModeratorService.GetModeratorsByCommunityAsync(communityId);
		}

		[Authorize]
		[Route("getCandidates/{communityId}")]
		[HttpGet]
		public async Task<ActionResult<UserViewModel[]>> GetModeratorCandidatesByCommunity(int communityId)
		{
			var doesCommunityExists = await _communityService.DoesExistAsync(communityId);
			if (!doesCommunityExists)
			{
				return BadRequest("Community does not exists");
			}
			return await _communityModeratorService.GetModeratorCandidatesByCommunityAsync(communityId);
		}

		[Authorize]
		[Route("approveCandidate/{accountId}/{communityId}")]
		[HttpPost]
		public async Task<IActionResult> ApproveCandidate(string accountId, int communityId)
		{
			var doesAccountExists = await _accountService.DoesExistAsync(accountId);
			if (!doesAccountExists)
			{
				return BadRequest("Account does not exists");
			}
			var doesCommunityExists = await _communityService.DoesExistAsync(communityId);
			if (!doesCommunityExists)
			{
				return BadRequest("Community does not exists");
			}
			await _communityModeratorService.ApproveCandidateAsync(accountId, communityId);
			return Ok();
		}

		[Authorize]
		[Route("removeModerator/{accountId}/{communityId}")]
		[HttpPost]
		public async Task<IActionResult> RemoveModerator(string accountId, int communityId)
		{
			var doesAccountExists = await _accountService.DoesExistAsync(accountId);
			if (!doesAccountExists)
			{
				return BadRequest("Account does not exists");
			}
			var doesCommunityExists = await _communityService.DoesExistAsync(communityId);
			if (!doesCommunityExists)
			{
				return BadRequest("Community does not exists");
			}
			await _communityModeratorService.RemoveModeratorAsync(accountId, communityId);
			return Ok();
		}

	}
}
