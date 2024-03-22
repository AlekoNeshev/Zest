using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Zest.Services.Hubs;
using Zest.Services.Infrastructure.Interfaces;

namespace Zest.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class LikesController : ControllerBase
	{
		private readonly ILikeService _likeService;
		private readonly IHubContext<LikesHub> _likesHubContext;

		public LikesController(ILikeService likeService, IHubContext<LikesHub> likesHubContext)
		{
			_likeService = likeService;
			_likesHubContext = likesHubContext;
		}

		[Route("add/post/{postId}/comment/{commentId}/value/{value}")]
		[HttpPost]
		public async Task<ActionResult> Add(int postId, int commentId, bool value)
		{
			var user = User.Claims;
			var likerId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

			if (postId != 0 && commentId == 0)
			{
				await _likeService.AddLikeToPost(likerId, postId, value);
			}
			else if (commentId != 0)
			{
				await _likeService.AddLikeToComment(likerId, commentId, value);
			}

			if (commentId == 0)
			{
				await _likesHubContext.Clients.Group(postId.ToString()).SendAsync("PostLiked", postId);
			}
			else if (commentId != 0)
			{
				await _likesHubContext.Clients.Group(("pd-" + postId.ToString())).SendAsync("CommentLiked", commentId);
			}

			return Ok();
		}

		[Route("remove/like/{likeId}/{postId}/{commentId}")]
		[HttpDelete]
		public async Task<ActionResult> Remove(int likeId, int postId, int commentId)
		{
			var user = User.Claims;
			var likerId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

			await _likeService.RemoveLike(likeId);

			if (commentId == 0)
			{
				await _likesHubContext.Clients.Group(postId.ToString()).SendAsync("PostLiked", postId);
			}
			else if (commentId != 0)
			{
				await _likesHubContext.Clients.Group(("pd-" + postId.ToString())).SendAsync("CommentLiked", commentId);
			}
		
			return Ok();
		}
	}
}
