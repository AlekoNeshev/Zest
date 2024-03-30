using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Zest.Services.Infrastructure.Services;

namespace Zest.Services.Hubs
{
	[AllowAnonymous]
	public class MessageHub : Hub
	{
		public MessageHub()
		{

		}
		
	}
}
