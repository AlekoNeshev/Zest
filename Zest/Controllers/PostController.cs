using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Zest.Services.Hubs;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
		private readonly IPostService _postService;
		private readonly IHubContext<DeleteHub> _hubContext;
		private readonly ICommunityService _communityService;
		public PostController(IPostService postService, IHubContext<DeleteHub> hubContext, ICommunityService communityService)
		{
			_postService = postService;
			_communityService = communityService;
			_hubContext = hubContext;
		}

		[Route("{id}")]
		[HttpGet]
		public async Task<ActionResult<PostViewModel>> Find(int id)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var post = await _postService.FindAsync(id, accountId);
			if (post == null)
			{
				return BadRequest("Post does not exist");
			}
			return post;
		}

		[Route("add/{title}/community/{communityId}")]
		[HttpPost]
		public async Task<ActionResult> Add(string title, [FromBody] string text, int communityId)
		{
			var user = User.Claims;
			var publisherId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var doesCommunityExists = await _communityService.DoesExistAsync(communityId);
			if (!doesCommunityExists)
			{
				return BadRequest("Community does not exists");
			}

			var post = await _postService.AddAsync(title, text, publisherId, communityId);
			return Ok(post.Id);
		}

		[Route("remove/{postId}")]
		[HttpPut]
		public async Task<ActionResult> Remove(int postId)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var post = await _postService.FindAsync(postId, accountId);

			if (post == null)
			{
				return BadRequest("Post does not exist!");
			}

			await _postService.RemoveAsync(postId);
			await _hubContext.Clients.Group("pdd-" +  postId.ToString()).SendAsync("PostDeleted", postId);
			return Ok();
		}

		[Route("getByDate/{lastDate}/{communityId}/{takeCount}")]
		[HttpGet]
		public async Task<ActionResult<PostViewModel[]>> GetByDate([FromRoute] DateTime lastDate, int communityId, int takeCount)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			if (communityId !=0)
			{
				var doesCommunityExists = await _communityService.DoesExistAsync(communityId);
				if (!doesCommunityExists)
				{
					return BadRequest("Community does not exists");
				}
			}
			var posts = await _postService.GetByDateAsync(accountId, lastDate, communityId, takeCount);
			
			return posts;
		}

		[Route("getByCommunity/{communityId}")]
		[HttpGet]
		public async Task<ActionResult<PostViewModel[]>> GetByCommunity(int communityId)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

			var doesCommunityExists = await _communityService.DoesExistAsync(communityId);
			if (!doesCommunityExists)
			{
				return BadRequest("Community does not exists");
			}
			var posts = await _postService.GetByCommunityAsync(communityId);
			
			return posts;
		}

		[Route("getBySearch/{search}/{takeCount}/{communityId}")]
		[HttpPost]
		public async Task<ActionResult<PostViewModel[]>> GetBySearch(string search, int takeCount, int communityId, [FromBody] int[]? skipIds)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrWhiteSpace(search))
			{
				return BadRequest("Search is empty!");

			}
			if (communityId !=0)
			{
				var doesCommunityExists = await _communityService.DoesExistAsync(communityId);
				if (!doesCommunityExists)
				{
					return BadRequest("Community does not exists");
				}
			}
			
			var posts = await _postService.GetBySearchAsync(search, accountId, takeCount, communityId, skipIds);
			
			return posts;
		}
		[Route("getByTrending/{takeCount}/{communityId}")]
		[HttpPost]
		public async Task<ActionResult<PostViewModel[]>> GetByTrending(int takeCount, int communityId,[FromBody] int[]? skipIds)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			if (communityId !=0)
			{
				var doesCommunityExists = await _communityService.DoesExistAsync(communityId);
				if (!doesCommunityExists)
				{
					return BadRequest("Community does not exists");
				}
			}
			var posts = await _postService.GetTrendingAsync(skipIds, takeCount, accountId, communityId);
			
			return posts.ToArray();
		}
		[Route("getByFollowed/{takeCount}")]
		[HttpPost]
		public async Task<ActionResult<PostViewModel[]>> GetByFollowed(int takeCount, [FromBody] int[]? skipIds)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var posts = await _postService.GetFollowedPostsAsync(skipIds, takeCount, accountId);
			
			return posts.ToArray();
		}
	}
}
