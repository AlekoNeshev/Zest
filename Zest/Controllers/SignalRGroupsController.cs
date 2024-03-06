using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using Zest.Hubs;
using Zest.Services;

namespace Zest.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class SignalRGroupsController : ControllerBase
	{
        private readonly IHubContext<LikesHub> _likesHubContext;
		private readonly IHubContext<MessageHub> _messageHubContext;
		private readonly IHubContext<CommentsHub> _commentsHubContext;
		private readonly SignaRGroupsPlaceholder _signaRGroupsPlaceholder;
		public SignalRGroupsController(IHubContext<LikesHub> likesHubContext, IHubContext<MessageHub> messageHubContext, IHubContext<CommentsHub> commentsHubContext, SignaRGroupsPlaceholder signaRGroupsPlaceholder)

		{
            this._likesHubContext = likesHubContext;
			this._messageHubContext = messageHubContext;
			this._commentsHubContext = commentsHubContext;
			this._signaRGroupsPlaceholder = signaRGroupsPlaceholder;
        }
		
        [HttpPost]
		[Route("addConnectionToGroup/{connectionId}")]
		public async Task<ActionResult> AddConnectionToGroup(string connectionId, [FromBody]string[]? groupsId)
		{
			foreach (var item in groupsId)
			{
				if (item.Contains("chat"))
				{
					
					await _messageHubContext.Groups.AddToGroupAsync(connectionId, item);
				}
				else if (item.Contains("message"))
				{
					await _commentsHubContext.Groups.AddToGroupAsync(connectionId, item);
					
				}
				else if (item.Contains("pd"))
				{
					await _likesHubContext.Groups.AddToGroupAsync(connectionId, item);
				}
				else
				{
					await _likesHubContext.Groups.AddToGroupAsync(connectionId, item);
				}
				await _signaRGroupsPlaceholder.AddUserToGroup(connectionId, item);
			}
			return Ok();
		
		}
		[HttpPost]
		[Route("removeConnectionToGroup/{connectionId}")]
		public async Task<ActionResult> RemoveConnectionFromAllGroups(string connectionId)
		{
			var groups = await _signaRGroupsPlaceholder.RetrieveGroups(connectionId);
			foreach (var group in groups)
			{
				await _likesHubContext.Groups.RemoveFromGroupAsync(connectionId, group);
				await _messageHubContext.Groups.RemoveFromGroupAsync(connectionId, group);
				await _commentsHubContext.Groups.RemoveFromGroupAsync(connectionId, group);
			}
			return Ok();
		}

	}
}
