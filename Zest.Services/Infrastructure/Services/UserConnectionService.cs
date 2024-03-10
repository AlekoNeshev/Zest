using Microsoft.AspNetCore.SignalR;
using Zest.Services.Hubs;

namespace Zest.Services.Infrastructure.Services
{
	public class UserConnectionService
	{
		private readonly IDictionary<string, HashSet<string>> _userConnections = new Dictionary<string, HashSet<string>>();
		private readonly IHubContext<LikesHub> _hubContext;
		public UserConnectionService(IHubContext<LikesHub> hubContext)
		{
			_hubContext = hubContext;
		}

		public void AddConnection(string userId, string connectionId)
		{
			lock (_userConnections)
			{
				if (!_userConnections.ContainsKey(userId))
				{
					_userConnections[userId] = new HashSet<string>();
				}
				_userConnections[userId].Add(connectionId);
			}
		}

		public void RemoveConnection(string userId, string connectionId)
		{
			lock (_userConnections)
			{
				if (_userConnections.ContainsKey(userId))
				{
					_userConnections[userId].Remove(connectionId);
					if (_userConnections[userId].Count == 0)
					{
						_userConnections.Remove(userId);
					}
				}
			}
		}


		public async Task SendNotificationToUser(string userId, int commentId)
		{
			if (_userConnections.ContainsKey(userId))
			{
				foreach (var connectionId in _userConnections[userId])
				{
					await SendNotificationToConnection(connectionId, commentId);
				}
			}
		}

		private async Task SendNotificationToConnection(string connectionId, int commentId)
		{
			await _hubContext.Clients.Client(connectionId).SendAsync("CommentLiked", commentId);
		}
	}

}
