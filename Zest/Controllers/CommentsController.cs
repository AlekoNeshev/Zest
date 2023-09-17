using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zest.DBModels;
using Zest.DBModels.Models;

namespace Zest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private ZestContext context;
        public CommentsController(ZestContext context)
        {
            this.context = context;
        }
        [HttpPost]
        public async Task<ActionResult> Add(int accountId, int postId, string text, int commentId = 0)
        {
            if (postId!=0 && commentId == 0)
            {
                context.Add(new Comment { AccountId = accountId, PostId = postId, Text = text, CreatedOn = DateTime.Now });
            }

            else if (postId!=0 && commentId != 0)
            {
                context.Add(new Comment { AccountId = accountId, PostId = postId, CommentId = commentId, Text = text, CreatedOn = DateTime.Now });
            }
            context.SaveChanges();
            return Ok();
        }

        
        [HttpDelete]
        public async Task<ActionResult> Remove(int accountId, int postId, int commentId)
        {
            Comment comment = new Comment();
            if (postId!=0 && commentId == 0)
            {
                comment = context.Comments.FirstOrDefault(c=>c.AccountId == accountId && c.PostId == postId);
               
            }
            else if (postId!=0 && commentId != 0)
            {
                comment = context.Comments.FirstOrDefault(c=>c.AccountId == accountId && c.CommentId == commentId && c.PostId == postId);
               
            }
            context.Remove(comment);
            context.SaveChanges();
            return Ok();
        }
    }
}
