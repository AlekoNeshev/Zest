using Microsoft.AspNetCore.SignalR;

using Zest.Services.Hubs;
using Zest.Services.Infrastructure.Interfaces;

namespace Zest.Services.Infrastructure.Services
{
	public class SignalRService : ISignaRService
	{
		private readonly IHubContext<LikesHub> _likesHubContext;
		private readonly IHubContext<MessageHub> _messageHubContext;
		private readonly IHubContext<DeleteHub> _deleteHubContext;
		private readonly SignaRGroupsPlaceholder _signaRGroupsPlaceholder;
		public SignalRService(IHubContext<LikesHub> likesHubContext, IHubContext<MessageHub> messageHubContext, IHubContext<DeleteHub> commentsHubContext, SignaRGroupsPlaceholder signaRGroupsPlaceholder) 
		{
			this._likesHubContext = likesHubContext;
			this._messageHubContext = messageHubContext;
			this._deleteHubContext = commentsHubContext;
			this._signaRGroupsPlaceholder = signaRGroupsPlaceholder;
		}
		public async Task AddConnectionToGroup(string connectionId,  string[]? groupsId)
		{
			foreach (var item in groupsId)
			{
				if (item.Contains("chat"))
				{

					await _messageHubContext.Groups.AddToGroupAsync(connectionId, item);
				}
				else if (item.Contains("pdd"))
				{
					await _deleteHubContext.Groups.AddToGroupAsync(connectionId, item);

				}
				else if (item.Contains("pdl"))
				{
					await _likesHubContext.Groups.AddToGroupAsync(connectionId, item);
				}
				else
				{
					await _likesHubContext.Groups.AddToGroupAsync(connectionId, item);
				}
				await _signaRGroupsPlaceholder.AddUserToGroup(connectionId, item);
			}
		}
		public async Task RemoveConnectionFromAllGroups(string connectionId)
		{
			var groups = await _signaRGroupsPlaceholder.RetrieveGroups(connectionId);
			foreach (var group in groups)
			{
				await _likesHubContext.Groups.RemoveFromGroupAsync(connectionId, group);
				await _messageHubContext.Groups.RemoveFromGroupAsync(connectionId, group);
				await _deleteHubContext.Groups.RemoveFromGroupAsync(connectionId, group);
			}
		
		}

	}
}
