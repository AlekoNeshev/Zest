using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Zest.Services.Infrastructure.Services;

namespace Zest.Services.Hubs;

[AllowAnonymous]
public class LikesHub : Hub
{
	public LikesHub()
	{
	

	}	

}
