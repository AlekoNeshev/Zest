using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class FollowersController : ControllerBase
    {
		private readonly IFollowerService _followerService;
		
		public FollowersController(IFollowerService followerService)
		{
			_followerService = followerService;
		}

		[Route("followers/find/{followerId}/{followedId}")]
		[HttpGet]
		public async Task<ActionResult<FollowerViewModel>> Find(string followerId, string followedId)
		{
			var follower = await _followerService.FindAsync(followerId, followedId);
			return follower;
		}

		[Route("add/followed/{followedId}")]
		[HttpPost]
		public async Task<ActionResult> Add(string followedId)
		{
			var user = User.Claims;
			var followerId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			await _followerService.AddAsync(followerId, followedId);
			return Ok();
		}

		[Route("delete/followed/{followedId}")]
		[HttpDelete]
		public async Task<ActionResult> Delete(string followedId)
		{
			var user = User.Claims;
			var followerId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			await _followerService.DeleteAsync(followerId, followedId);
			return Ok();
		}

		[Route("getFriends")]
		[HttpGet]
		public async Task<ActionResult<FollowerViewModel[]>> FindFriends()
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var followers = await _followerService.FindFriendsAsync(accountId);
			return followers;
		}
	}
}
