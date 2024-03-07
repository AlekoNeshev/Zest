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
		private readonly ICommunityModeratorService communityModeratorService;
		private readonly IMapper mapper;
		public CommunityModeratorsController(ICommunityModeratorService communityModeratorService, IMapper mapper)
		{
			this.communityModeratorService = communityModeratorService;
			this.mapper = mapper;
		}
		[Authorize]
		[Route("isModerator/{accountId}/{communityId}")]
		[HttpGet]
		public async Task<ActionResult<bool>> Find(string accountId, int communityId)
		{
			return await communityModeratorService.IsModeratorAsync(accountId, communityId);
		}

		[Authorize]
		[Route("add/{accountId}/{communityId}")]
		[HttpPost]
		public async Task<IActionResult> Add(string accountId, int communityId)
		{
			await communityModeratorService.AddModeratorAsync(accountId, communityId);
			return Ok();
		}

		[Authorize]
		[Route("getModerators/{communityId}")]
		[HttpGet]
		public async Task<UserViewModel[]> GetModeratorsByCommunity(int communityId)
		{
			return mapper.Map<UserViewModel[]>(await communityModeratorService.GetModeratorsByCommunityAsync(communityId));
		}

		[Authorize]
		[Route("getCandidates/{communityId}")]
		[HttpGet]
		public async Task<UserViewModel[]> GetModeratorCandidatesByCommunity(int communityId)
		{
			return mapper.Map<UserViewModel[]>(await communityModeratorService.GetModeratorCandidatesByCommunityAsync(communityId));
		}

		[Authorize]
		[Route("approveCandidate/{accountId}/{communityId}")]
		[HttpPost]
		public async Task<IActionResult> ApproveCandidate(string accountId, int communityId)
		{
			await communityModeratorService.ApproveCandidateAsync(accountId, communityId);
			return Ok();
		}

		[Authorize]
		[Route("removeModerator/{accountId}/{communityId}")]
		[HttpPost]
		public async Task<IActionResult> RemoveModerator(string accountId, int communityId)
		{
			await communityModeratorService.RemoveModeratorAsync(accountId, communityId);
			return Ok();
		}

	}
}
