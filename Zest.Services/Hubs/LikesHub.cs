using Microsoft.AspNetCore.SignalR;
using Zest.Services.Infrastructure.Services;

namespace Zest.Services.Hubs;


public class LikesHub : Hub
{
	private readonly UserConnectionService _notificationService;
	private readonly SignaRGroupsPlaceholder _likesConnectionService;
	public LikesHub(UserConnectionService notificationService, SignaRGroupsPlaceholder likesHubConnectionService)
	{
		_notificationService = notificationService;
		_likesConnectionService = likesHubConnectionService;

	}

	public override async Task OnConnectedAsync()
	{
		var connectionId = Context.ConnectionId;
		var uniqueProperty = Context.GetHttpContext().Request.Headers["userId"];


		_notificationService.AddConnection(uniqueProperty, connectionId);
		//_likesConnectionService.AddUserToGroup(uniqueProperty, uniquePost, connectionId);


		await base.OnConnectedAsync();
	}
	public async Task SignalLike(int postId, int likes = 1)
	{
		await Clients.All.SendAsync("PostLiked", likes);
	}

}
