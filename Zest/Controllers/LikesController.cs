using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zest.DBModels;
using Zest.DBModels.Models;

namespace Zest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private ZestContext context;
        public LikesController(ZestContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Add(int likerId, int postId, int commentId, bool value)
        {
            context.Add(new Like
            { 
                AccountId = likerId,
                PostId = postId, 
                CommentId = commentId, 
                Value = value,
                CreatedOn = DateTime.Now
            });
            context.SaveChanges();
            return Ok();
        }
        [HttpDelete]
        public async Task<ActionResult> Remove(int likerId, int postId, int commentId)
        {
            Like like = context.Likes.FirstOrDefault(l => l.AccountId == likerId && l.PostId == postId && l.CommentId == commentId);
            context.Remove(like);
            context.SaveChanges();
            return Ok();
        }
    }
}
