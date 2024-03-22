﻿using Microsoft.AspNetCore.SignalR;
using Zest.Services.Infrastructure.Services;

namespace Zest.Services.Hubs
{
	public class MessageHub : Hub
	{
		private readonly UserConnectionService _notificationService;
		public MessageHub(UserConnectionService notificationService)
		{
			_notificationService = notificationService;

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