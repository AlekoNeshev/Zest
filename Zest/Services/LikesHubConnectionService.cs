using Microsoft.AspNetCore.SignalR;
using Zest.Hubs;

namespace Zest.Services
{
	public class LikesHubConnectionService
	{
		private readonly IDictionary<string, HashSet<string>> _userGroups = new Dictionary<string, HashSet<string>>();
		private readonly IHubContext<LikesHub> _hubContext;

		public LikesHubConnectionService(IHubContext<LikesHub> hubContext)
		{
			_hubContext = hubContext;
		}

		public async void AddUserToGroup(string userId, string groupName, string connectionId)
		{
			lock (_userGroups)
			{
				if (!_userGroups.ContainsKey(userId))
				{
					_userGroups[userId] = new HashSet<string>();
				}
				_userGroups[userId].Add(groupName);
			   
			}
			await _hubContext.Groups.AddToGroupAsync(connectionId, groupName);
		}

		public void RemoveUserFromGroup(string userId, string groupName)
		{
			lock (_userGroups)
			{
				if (_userGroups.ContainsKey(userId))
				{
					_userGroups[userId].Remove(groupName);
					if (_userGroups[userId].Count == 0)
					{
						_userGroups.Remove(userId);
					}
				}
			}
		}

		public async Task SendNotificationToUser(string userId, string commentId)
		{
			if (_userGroups.ContainsKey(userId))
			{
				foreach (var groupName in _userGroups[userId])
				{
					await SendNotificationToGroup(groupName, commentId);
				}
			}
		}

		private async Task SendNotificationToGroup(string groupName, string commentId)
		{
			await _hubContext.Clients.Group(groupName).SendAsync("CommentLiked", commentId);
		}
	}
}
