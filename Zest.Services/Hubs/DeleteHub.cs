using Microsoft.AspNetCore.SignalR;

using Zest.Services.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace Zest.Services.Hubs
{
	[AllowAnonymous]
	public class DeleteHub : Hub
	{
		public DeleteHub()
		{
			
		}
		
	}
}
