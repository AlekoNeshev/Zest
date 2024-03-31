using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Zest.Services.Hubs;
using Zest.Services.Infrastructure.Interfaces;

namespace Zest.Controllers
{
	[Authorize]
	[Route("Zest/[controller]")]
	[ApiController]
	public class LikeController : ControllerBase
	{
		private readonly ILikeService _likeService;
		private readonly IHubContext<LikesHub> _likesHubContext;
		private readonly IPostService _postService;
		private readonly ICommentsService _commentsService;
		public LikeController(ILikeService likeService, IHubContext<LikesHub> likesHubContext, IPostService postService, ICommentsService commentsService)
		{
			_likeService = likeService;
			_likesHubContext = likesHubContext;
			_postService=postService;
			_commentsService=commentsService;
		}

		[Route("add/post/{postId}/comment/{commentId}/value/{value}")]
		[HttpPost]
		public async Task<IActionResult> Add(int postId, int commentId, bool value)
		{
			var likerId = User.Id();

			var doesPostExist = await _postService.DoesExist(postId);
			if (!doesPostExist)
			{
				return BadRequest("Post does not exist!");
			}

			if (postId != 0 && commentId == 0)
			{
				
				await _likeService.AddLikeToPostAsync(likerId, postId, value);
			}
			else if (commentId != 0)
			{
				var doesCommentExist = await _commentsService.DoesExist(commentId);
				if (!doesCommentExist)
				{
					return BadRequest("Comment does not exist");
				}
				await _likeService.AddLikeToCommentAsync(likerId, commentId, value);
			}

			if (commentId == 0)
			{
				await _likesHubContext.Clients.Group(postId.ToString()).SendAsync("PostLiked", postId);
			}
			else if (commentId != 0)
			{
				await _likesHubContext.Clients.Group(("pdl-" + postId.ToString())).SendAsync("CommentLiked", commentId);
			}

			return Ok();
		}

		[Route("remove/like/{likeId}/{postId}/{commentId}")]
		[HttpDelete]
		public async Task<ActionResult> Remove(int likeId, int postId, int commentId)
		{
			var likerId = User.Id();
			var doesPostExist = await _postService.DoesExist(postId);
			if (!doesPostExist)
			{
				return BadRequest("Post does not exist!");
			}
			if(commentId !=  0)
			{
				var doesCommentExist = await _commentsService.DoesExist(commentId);
				if (!doesCommentExist)
				{
					return BadRequest("Comment does not exist");
				}
			}
			await _likeService.RemoveLikeAsync(likeId);

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
