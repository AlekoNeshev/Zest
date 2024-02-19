using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Services;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private ZestContext context;
        private IMapper mapper;
        private SignaRGroupsPlaceholder connectionService;
        public PostController(ZestContext context, IMapper mapper, SignaRGroupsPlaceholder likesHubConnectionService)
        {
            this.context = context;
            this.mapper = mapper;
            this.connectionService = likesHubConnectionService;
        }
        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult<PostViewModel>> Find(int id)
        {
           // Console.WriteLine(context.Posts.Find(id).Account.Username);
            return mapper.Map<PostViewModel>(context.Posts.Find(id));
        }

        [Route("add/{title}/account/{publisherId}/community/{communityId}")]
        [HttpPost]
        public async Task<ActionResult> Add(string title,[FromBody] string text, int publisherId, int communityId)
        {
            var post = context.Add(new  Post 
            { 
                Title = title, 
                Text = text,
                AccountId = publisherId,
                CommunityId = communityId,
                CreatedOn = DateTime.Now,
            });
			await context.SaveChangesAsync();
            var postId = post.Property<int>("Id").CurrentValue;
			return Ok(postId);
		}
		[Route("remove/{postId}")]
		[HttpPut]
		public async Task<ActionResult> Remove(int postId)
		{
			Post post = context.Posts.Find(postId);
           
            if (post == null)
            {
                return BadRequest();
            }
			post.IsDeleted = true;
			context.Posts.Update(post);
			context.SaveChanges();
			return Ok();
		}
        [Route("getByDate/{accountId}/{lastDate}/{minimumSkipCount}/{takeCount}")]
        [HttpGet]
        public async Task<ActionResult<PostViewModel[]>> GetByDate(int accountId, [FromRoute] DateTime lastDate, int minimumSkipCount,int takeCount)
        {
            var query = context.Posts.OrderByDescending(p => p.CreatedOn).Skip(minimumSkipCount).Where(x=>x.CreatedOn < lastDate).Take(takeCount).ToArray();
           
			return mapper.Map<PostViewModel[]>(context.Posts.OrderByDescending(x => x.CreatedOn).ToArray());
        }
        [Route("getByCommunity/{accountId}/{communityId}")]
        [HttpGet]
        public async Task<ActionResult<PostViewModel[]>> GetByCommunity(int accountId, int communityId)
        {
            return mapper.Map<PostViewModel[]>(context.Posts.Where(x=>x.CommunityId==communityId).OrderByDescending(x => x.CreatedOn).ToArray());
        }
        [Route("getBySearch/{accountId}/{search}")]
        [HttpGet]
        public async Task<ActionResult<PostViewModel[]>> GetBySearch(int accountId, string search)
        {
			return mapper.Map<PostViewModel[]>(context.Posts.OrderByDescending(x=>x.Title.Contains(search)).ThenByDescending(x=>x.Text.Contains(search)).ThenByDescending(x=>x.CreatedOn).ToArray());
		}
    }
}
