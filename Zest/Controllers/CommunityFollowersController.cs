﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zest.Services.Infrastructure.Interfaces;

namespace Zest.Controllers
{
	[Authorize]
	[Route("Zest/[controller]")]
	[ApiController]
	public class CommunityFollowersController : ControllerBase
	{
		private readonly ICommunityFollowerService _communityFollowerService;

		public CommunityFollowersController(ICommunityFollowerService communityFollowerService)
		{
			_communityFollowerService = communityFollowerService;
		}

		[HttpGet]
		public async Task<ActionResult<bool>> DoesExist(int communityId)
		{
			var accountId = User.Id();
			bool doesExist = await _communityFollowerService.DoesExistAsync(accountId, communityId);
			return doesExist;
		}

		[Route("account/add/community/{communityId}")]
		[HttpPost]
		public async Task<IActionResult> Add(int communityId)
		{
			var accountId = User.Id();
			bool doesFolloweshipExist = await _communityFollowerService.DoesExistAsync(accountId, communityId);
			if(doesFolloweshipExist)
			{
				return BadRequest("Followship already exists");
			}
			await _communityFollowerService.AddAsync(accountId, communityId);
			return Ok();
		}

		[Route("account/delete/community/{communityId}")]
		[HttpDelete]
		public async Task<IActionResult> Delete(int communityId)
		{
			var accountId = User.Id();
			bool doesFolloweshipExist = await _communityFollowerService.DoesExistAsync(accountId, communityId);
			if (!doesFolloweshipExist)
			{
				return BadRequest("Followship does not exist");
			}
			await _communityFollowerService.DeleteAsync(accountId, communityId);
			return Ok();
		}
	}
}
