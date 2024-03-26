using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Services.Hubs;
using Zest.Services.Infrastructure.Interfaces;
using Zest.Services.Infrastructure.Services;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
		private readonly ICommentsService _commentsService;
		private readonly IHubContext<CommentsHub> _commentsHubContext;


		public CommentsController(ICommentsService commentsService, IHubContext<CommentsHub> hubContext)
		{
			_commentsService = commentsService;
			_commentsHubContext = hubContext;
			
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
		public async Task<ActionResult> Add(int postId, [FromBody] string text, int commentId = 0)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

			var newComment = await _commentsService.AddAsync(accountId, postId, text, commentId);
			
			if(commentId == 0)
			{
				var returnId = newComment.Property<int>("Id").CurrentValue;
				return Ok(returnId);
			}
			else
			{
				var returnIds = new int[] { newComment.Property<int>("Id").CurrentValue, (int)newComment.Property<int?>("CommentId").CurrentValue };
				return Ok(returnIds);
			}
			
			
			return BadRequest();
		}

		[Route("remove/{commentId}")]
		[HttpPut]
		public async Task<ActionResult> Remove(int commentId)
		{		
			var postId = await _commentsService.RemoveAsync(commentId);
			await _commentsHubContext.Clients.Group(("comment-" + postId.ToString())).SendAsync("CommentDeleted", commentId);
			return Ok(commentId);
		}

		[Route("getCommentsByPost/{postId}/{lastDate}/{takeCount}")]
		[HttpGet]
		public async Task<ActionResult<CommentViewModel[]>> GetCommentsByPost(int postId, [FromRoute] DateTime lastDate, int takeCount)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var comments = await _commentsService.GetCommentsByPostIdAsync(postId, lastDate, takeCount, accountId);
			
			return comments;
		}
		[Route("getByTrending/{takeCount}/{postId}")]
		[HttpPost]
		public async Task<ActionResult<CommentViewModel[]>> GetByTrending(int takeCount, int postId, [FromBody] int[]? skipIds)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var posts = await _commentsService.GetTrending(skipIds, takeCount, accountId, postId);

			return posts.ToArray();
		}
	}

}
