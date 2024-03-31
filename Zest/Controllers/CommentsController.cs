using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Zest.Services.Hubs;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
		private readonly ICommentsService _commentsService;
		private readonly IPostService _postService;
		private readonly IHubContext<DeleteHub> _deleteHubContext;


		public CommentsController(ICommentsService commentsService, IHubContext<DeleteHub> hubContext, IPostService postService)
		{
			_commentsService = commentsService;
			_deleteHubContext = hubContext;
			_postService=postService;
		}

		[HttpGet]
		[Route("{id}")]
		public async Task<ActionResult<CommentViewModel>> Find(int id)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var comment = await _commentsService.FindAsync(id, accountId);
			return comment;
		}

		[Route("add/post/{postId}/comment/{commentId}")]
		[HttpPost]
		public async Task<IActionResult> Add(int postId, [FromBody] string text, int commentId = 0)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

			var doesPostExist = await _postService.DoesExist(postId);
			if (!doesPostExist)
			{
				return BadRequest("Post does not exist!");
			}
			if(commentId != 0)
			{
				var doesCommentExist = await _commentsService.DoesExist(commentId);
				if (!doesCommentExist)
				{
					return BadRequest("Comment does not exist");
				}
			}

			var newComment = await _commentsService.AddAsync(accountId, postId, text, commentId);
			
			if(commentId == 0)
			{
				var returnId = newComment.Id;
				return Ok(returnId);
			}
			else
			{
				var returnIds = new int[] { newComment.Id, (int)newComment.CommentId };
				return Ok(returnIds);
			}
			
			
			
		}

		[Route("remove/{commentId}/{postId}")]
		[HttpPut]
		public async Task<IActionResult> Remove(int commentId, int postId)
		{
			var doesCommentExist = await _commentsService.DoesExist(commentId);
			if(!doesCommentExist)
			{
				return BadRequest("Comment does not exist");
			}
			await _commentsService.RemoveAsync(commentId);
			await _deleteHubContext.Clients.Group(("pdd-" + postId.ToString())).SendAsync("CommentDeleted", commentId);
			return Ok(commentId);
		}

		[Route("getCommentsByPost/{postId}/{lastDate}/{takeCount}")]
		[HttpGet]
		public async Task<ActionResult<CommentViewModel[]>> GetCommentsByPost(int postId, [FromRoute] DateTime lastDate, int takeCount)
		{
			var doesPostExist = await _postService.DoesExist(postId);
			if (!doesPostExist)
			{
				return BadRequest("Post does not exist!");
			}
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var comments = await _commentsService.GetCommentsByPostIdAsync(postId, lastDate, takeCount, accountId);
			
			return comments;
		}
		[Route("getByTrending/{takeCount}/{postId}")]
		[HttpPost]
		public async Task<ActionResult<CommentViewModel[]>> GetByTrending(int takeCount, int postId, [FromBody] int[]? skipIds)
		{
			var doesPostExist = await _postService.DoesExist(postId);
			if (!doesPostExist)
			{
				return BadRequest("Post does not exist!");
			}
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var posts = await _commentsService.GetTrendingCommentsAsync(skipIds, takeCount, accountId, postId);

			return posts.ToArray();
		}
	}

}
