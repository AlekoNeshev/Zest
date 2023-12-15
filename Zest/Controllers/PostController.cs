using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private ZestContext context;
        private IMapper mapper;
        public PostController(ZestContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult<PostViewModel>> Find(int id)
        {
           // Console.WriteLine(context.Posts.Find(id).Account.Username);
            return mapper.Map<PostViewModel>(context.Posts.Find(id));
        }

        [Route("remove/{title}/account/{publisherId}/community/{communityId}")]
        [HttpPost]
        public async Task<ActionResult> Add(string title,[FromBody] string text, int publisherId, int communityId)
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
        [Route("getByDate")]
        [HttpGet]
        public async Task<ActionResult<PostViewModel[]>> GetByDate()
        {
            return mapper.Map<PostViewModel[]>(context.Posts.OrderByDescending(x => x.CreatedOn).ToArray());
        }
        [Route("getByCommunity/{communityId}")]
        [HttpGet]
        public async Task<ActionResult<PostViewModel[]>> GetByCommunity(int communityId)
        {
            return mapper.Map<PostViewModel[]>(context.Posts.Where(x=>x.CommunityId==communityId).OrderByDescending(x => x.CreatedOn).ToArray());
        }
    }
}
