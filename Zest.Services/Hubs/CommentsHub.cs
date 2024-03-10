using Microsoft.AspNetCore.SignalR;

using Zest.Services.Infrastructure.Services;
using Microsoft.AspNetCore.Http;

namespace Zest.Services.Hubs
{
	public class CommentsHub : Hub
	{
		private readonly UserConnectionService _notificationService;
		public CommentsHub(UserConnectionService userConnectionService)
		{
			_notificationService = userConnectionService;
		}
		public override async Task OnConnectedAsync()
		{
			var connectionId = Context.ConnectionId;
			
			var uniqueProperty = Context.GetHttpContext().Request.Headers["userId"];

			_notificationService.AddConnection(uniqueProperty, connectionId);

			await base.OnConnectedAsync();
		}
	}
}
