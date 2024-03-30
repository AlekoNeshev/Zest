using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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
    public class FollowersController : ControllerBase
    {
		private readonly IFollowerService _followerService;
		private readonly IAccountService _accountService;
		public FollowersController(IFollowerService followerService, IAccountService accountService)
		{
			_followerService = followerService;
			_accountService=accountService;
		}

		[Route("followers/find/{followerId}/{followedId}")]
		[HttpGet]
		public async Task<ActionResult<BaseAccountViewModel>> Find(string followerId, string followedId)
		{
			var follower = await _followerService.FindAsync(followerId, followedId);
			return follower;
		}

		[Route("add/followed/{followedId}")]
		[HttpPost]
		public async Task<IActionResult> Add(string followedId)
		{
			var user = User.Claims;
			var followerId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var doesAccountExists = await _accountService.DoesExistAsync(followedId);
			if (!doesAccountExists)
			{
				return BadRequest("Account does not exists");
			}
			await _followerService.AddAsync(followerId, followedId);
			return Ok();
		}

		[Route("delete/followed/{followedId}")]
		[HttpDelete]
		public async Task<IActionResult> Delete(string followedId)
		{
			var user = User.Claims;
			var followerId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var doesAccountExists = await _accountService.DoesExistAsync(followedId);
			if (!doesAccountExists)
			{
				return BadRequest("Account does not exists");
			}
			await _followerService.DeleteAsync(followerId, followedId);
			return Ok();
		}

		[Route("getFriends/{takeCount}/{skipCount}")]
		[HttpGet]
		public async Task<ActionResult<BaseAccountViewModel[]>> FindFriends(int takeCount, int skipCount = 0)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var followers = await _followerService.FindFriendsAsync(accountId, takeCount, skipCount);
			return followers;
		}
		[Route("getBySearch/{search}/{takeCount}")]
		[HttpPost]
		public async Task<ActionResult<BaseAccountViewModel[]>> GetBySearch(string search, int takeCount, [FromBody] string[]? skipIds)
		{
			if (string.IsNullOrWhiteSpace(search))
			{
				return BadRequest("Search is empty!");

			}
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var followers = await _followerService.GetBySearchAsync(search, accountId, takeCount, skipIds);

			return followers;
		}
	}
}
