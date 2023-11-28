using Microsoft.AspNetCore.SignalR;

namespace Zest.Hubs;


public class LikesHub : Hub
{

    public async Task SignalLike(int postId, int likes = 1)
    {
        await Clients.All.SendAsync("PostLiked", likes);
    }

}
