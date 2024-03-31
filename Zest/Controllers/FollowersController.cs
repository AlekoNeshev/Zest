using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
	[Authorize]
	[Route("Zest/[controller]")]
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
			var followerId = User.Id();
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
			var followerId = User.Id();
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
			var accountId = User.Id();
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
			var accountId = User.Id();
			var followers = await _followerService.GetBySearchAsync(search, accountId, takeCount, skipIds);

			return followers;
		}
	}
}
