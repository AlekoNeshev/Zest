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
		private readonly LikesHubConnectionService _likesHubConnectionService;
        public SignalRGroupsController(IHubContext<LikesHub> likesHubContext, LikesHubConnectionService likesHubConnectionService)
        {
            this._likesHubContext = likesHubContext;
			this._likesHubConnectionService = likesHubConnectionService;
        }
        [HttpPost]
		[Route("addConnectionToGroup/{connectionId}")]
		public async Task<ActionResult> AddConnectionToGroup(string connectionId, [FromBody]string[] groupsId)
		{
			foreach (var item in groupsId)
			{
				await _likesHubContext.Groups.AddToGroupAsync(connectionId, item);
			}
			return Ok();
		
		}

	}
}
