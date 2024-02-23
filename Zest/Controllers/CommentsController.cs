using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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
				var comment = await context.AddAsync(new Comment { AccountId = accountId, PostId = postId, Text = text, CreatedOn = DateTime.Now });
				context.SaveChanges();
				var returnId = comment.Property<int>("Id").CurrentValue;
                return Ok(returnId);
			}

            else if (postId!=0 && commentId != 0)
            {
				var comment = await context.AddAsync(new Comment { AccountId = accountId, PostId = postId, CommentId = commentId, Text = text, CreatedOn = DateTime.Now });
				context.SaveChanges();
				var returnId =new int[] { comment.Property<int>("Id").CurrentValue, (int)comment.Property<int?>("CommentId").CurrentValue };
                return Ok(returnId);
			}
            
            //await hubContext.Clients.Group("message-" + postId.ToString()).SendAsync("CommentPosted");
            return BadRequest();
        }

        [Route("remove/{commentId}")]
        [HttpPut]
        public async Task<ActionResult> Remove(int commentId)
        {
            Comment comment = new Comment();

            comment = context.Comments.FirstOrDefault(c => c.Id == commentId);
            if (comment == null)
            {
                return BadRequest();
            }
            comment.IsDeleted = true;
            context.Update(comment);
            await context.SaveChangesAsync();
           // await hubContext.Clients.All.SendAsync("CommentPosted");
            return Ok(commentId);
        }
        [Route("getCommentsByPost/{accountId}/{postId}")]
        [HttpGet]
        public async Task<ActionResult<CommentViewModel[]>> GetCommentsByPost(int accountId, int postId)
        {
            var comments = mapper.Map<CommentViewModel[]>(context.Comments.Include(x => x.Replies).ThenInclude(x => x.Replies).Where(x => x.PostId == postId && x.CommentId == null).ToArray());
          
			
			return comments;
        }
    }
}
