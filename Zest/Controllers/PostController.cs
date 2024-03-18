using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
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
    public class PostController : ControllerBase
    {
		private readonly IPostService _postService;
		private readonly IMapper _mapper;

		public PostController(IPostService postService, IMapper mapper)
		{
			_postService = postService;
			_mapper = mapper;
		}

		[Route("{id}")]
		[HttpGet]
		public async Task<ActionResult<PostViewModel>> Find(int id)
		{
			var post = await _postService.FindAsync(id);
			return _mapper.Map<PostViewModel>(post);
		}

		[Route("add/{title}/community/{communityId}")]
		[HttpPost]
		public async Task<ActionResult> Add(string title, [FromBody] string text, int communityId)
		{
			var user = User.Claims;
			var publisherId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var post = await _postService.AddAsync(title, text, publisherId, communityId);
			return Ok(post.Id);
		}

		[Route("remove/{postId}")]
		[HttpPut]
		public async Task<ActionResult> Remove(int postId)
		{
			var post = await _postService.FindAsync(postId);

			if (post == null)
			{
				return BadRequest();
			}

			await _postService.RemoveAsync(postId);
			return Ok();
		}

		[Route("getByDate/{lastDate}/{communityId}/{takeCount}")]
		[HttpGet]
		public async Task<ActionResult<PostViewModel[]>> GetByDate([FromRoute] DateTime lastDate, int communityId, int takeCount)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

			var posts = _mapper.Map<PostViewModel[]>(await _postService.GetByDateAsync(lastDate, communityId, takeCount));
			
			return posts;
		}

		[Route("getByCommunity/{communityId}")]
		[HttpGet]
		public async Task<ActionResult<PostViewModel[]>> GetByCommunity(int communityId)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var posts = _mapper.Map<PostViewModel[]>(await _postService.GetByCommunityAsync(communityId));
			
			return posts;
		}

		[Route("getBySearch/{search}")]
		[HttpGet]
		public async Task<ActionResult<PostViewModel[]>> GetBySearch(string search)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var posts = _mapper.Map<PostViewModel[]>(await _postService.GetBySearchAsync(search));
			
			return posts;
		}
		[Route("getByTrending/{takeCount}/{communityId}")]
		[HttpPost]
		public async Task<ActionResult<PostViewModel[]>> GetByTrending(int takeCount, int communityId,[FromBody] int[]? skipIds)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var posts = await _postService.GetTrending(skipIds, takeCount, communityId);
			
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
