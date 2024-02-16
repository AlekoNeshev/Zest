using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Zest.Hubs;
using Zest.Services;

namespace Zest.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SignalRGroupsController : ControllerBase
	{
        private readonly IHubContext<LikesHub> _likesHubContext;
		private readonly IHubContext<MessageHub> _messageHubContext;
		private readonly IHubContext<CommentsHub> _commentsHubContext;
		public SignalRGroupsController(IHubContext<LikesHub> likesHubContext, IHubContext<MessageHub> messageHubContext, IHubContext<CommentsHub> commentsHubContext)

		{
            this._likesHubContext = likesHubContext;
			this._messageHubContext = messageHubContext;
			this._commentsHubContext = commentsHubContext;
        }
        [HttpPost]
		[Route("addConnectionToGroup/{connectionId}")]
		public async Task<ActionResult> AddConnectionToGroup(string connectionId, [FromBody]string[] groupsId)
		{
			foreach (var item in groupsId)
			{
				if (item.Contains("chat"))
				{
					await _messageHubContext.Groups.AddToGroupAsync(connectionId, item);
				}
				else if (item.Contains("comment"))
				{
					await _commentsHubContext.Groups.AddToGroupAsync(connectionId, item);
				}
				else
				{
					await _likesHubContext.Groups.AddToGroupAsync(connectionId, item);
				}
			}
			return Ok();
		
		}

	}
}
