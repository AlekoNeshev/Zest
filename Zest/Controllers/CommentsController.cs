using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Hubs;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private ZestContext context;
        private IHubContext<CommentsHub> hubContext;
        private IMapper mapper;
        public CommentsController(ZestContext context, IMapper mapper, IHubContext<CommentsHub> hubContext)
        {
            this.context = context;
            this.mapper = mapper;
            this.hubContext = hubContext;
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CommentViewModel>> Find(int id)
        {
            Comment comment = await context.Comments.FindAsync(id);
            CommentViewModel vm= mapper.Map<CommentViewModel>(comment);
            return mapper.Map<CommentViewModel>(context.Comments.Find(id));
        }
        [Route("add/{accountId}/post/{postId}/comment/{commentId}")]
        [HttpPost]
        public async Task<ActionResult> Add(int accountId, int postId, [FromBody] string text, int commentId = 0)
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
            await hubContext.Clients.All.SendAsync("CommentPosted");
            return Ok();
        }

        [Route("remove/{commentId}")]
        [HttpDelete]
        public async Task<ActionResult> Remove(int commentId)
        {
            Comment comment = new Comment();

            comment = context.Comments.FirstOrDefault(c => c.Id == commentId);

            context.Remove(comment);
            context.SaveChanges();
            await hubContext.Clients.All.SendAsync("CommentPosted");
            return Ok();
        }
        [Route("getCommentsByPost/{postId}")]
        [HttpGet]
        public async Task<ActionResult<CommentViewModel[]>> GetCommentsByPost(int postId)
        {
            return mapper.Map<CommentViewModel[]>(context.Comments.Where(x => x.PostId == postId).ToArray());
        }
    }
}
