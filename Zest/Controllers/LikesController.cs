using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Hubs;
using Zest.Services;

namespace Zest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private ZestContext context;
        private IHubContext<LikesHub> _LikesHubCont;
        private UserConnectionService _UserConnectionService;
		private SignaRGroupsPlaceholder likesHubConnectionService;
		public LikesController(ZestContext context, IHubContext<LikesHub> lhc, UserConnectionService userConnectionService, SignaRGroupsPlaceholder likesHubConnectionService)
        {
            this.context = context;
            this._LikesHubCont = lhc;
            this._UserConnectionService = userConnectionService;
            this.likesHubConnectionService = likesHubConnectionService;
        }
        [Route("add/{likerId}/post/{postId}/comment/{commentId}/value/{value}")]
        [HttpPost]
        public async Task<ActionResult> Add(int likerId, int postId, int commentId, bool value)
        {
            if (postId!=0 && commentId == 0)
            {
                context.Add(new Like
                {
                    AccountId = likerId,
                    PostId = postId,
                 
                    Value = value,
                    CreatedOn = DateTime.Now
                });
            }
            else if(commentId !=0) 
            {
                context.Add(new Like
                {
                    AccountId = likerId,
                  
                    CommentId = commentId,
                    Value = value,
                    CreatedOn = DateTime.Now
                });
            }
            context.SaveChanges();
            if (commentId == 0)
                await _LikesHubCont.Clients.Groups(postId.ToString()).SendAsync("PostLiked", postId);
            else if (commentId != 0)
            {
				await _LikesHubCont.Clients.Groups(("pd-"+postId.ToString())).SendAsync("CommentLiked", commentId);
				
			}
            return Ok();
        }
        [Route("remove/{likerId}/post/{postId}/comment/{commentId}")]
        [HttpDelete]
        public async Task<ActionResult> Remove(int likerId, int postId, int commentId)
        {
            Like like = new Like();
            if (postId!=0)
            {
                 like = context.Likes.FirstOrDefault(l => l.AccountId == likerId && l.PostId == postId);
            }
            else if (commentId!=0)
            {
                like = context.Likes.FirstOrDefault(l => l.AccountId == likerId && l.CommentId == commentId);
            }
            context.Remove(like);
            context.SaveChanges();
            return Ok();
        }
    }
}
