using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zest.DBModels;
using Zest.DBModels.Models;

namespace Zest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private ZestContext context;
        public PostController(ZestContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Post>> Find(int id)
        {
            return context.Posts.Find(id);
        }
        [HttpPost]
        public async Task<ActionResult> Add(string title, string text, int publisherId, int communityId)
        {
            context.Add(new  Post 
            { 
                Title = title, 
                Text = text,
                AccountId = publisherId,
                CommunityId = communityId,
                CreatedOn = DateTime.Now,
            });
            context.SaveChanges();
            return Ok();
        }
    }
}
