using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Services;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
	[Authorize]
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
		[Route("add/{title}/community/{communityId}")]
        [HttpPost]
        public async Task<ActionResult> Add(string title,[FromBody] string text, int communityId)
        {
			var user = User.Claims;
			var publisherId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
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
        
        [Route("getByDate/{lastDate}/{minimumSkipCount}/{takeCount}")]
        [HttpGet]
		public async Task<ActionResult<PostViewModel[]>> GetByDate([FromRoute] DateTime lastDate, int minimumSkipCount,int takeCount)
        {
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

			var posts = mapper.Map<PostViewModel[]>( context.Posts.OrderByDescending(p => p.CreatedOn).Skip(minimumSkipCount).Where(x=>x.CreatedOn < lastDate).Take(takeCount).ToArray());
			foreach (var item in posts)
			{
				item.IsOwner = context.Posts.Where(x => x.Id == item.Id && x.AccountId == accountId).FirstOrDefault() != null;
			}
            return posts;
        }
        [Route("getByCommunity/{communityId}")]
        [HttpGet]
        public async Task<ActionResult<PostViewModel[]>> GetByCommunity(int communityId)
        {
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var posts = mapper.Map<PostViewModel[]>(context.Posts.Where(x => x.CommunityId==communityId).OrderByDescending(x => x.CreatedOn).ToArray());
            foreach (var item in posts)
            {
                item.IsOwner = context.Posts.Where(x => x.Id == item.Id && x.AccountId == accountId).FirstOrDefault() != null;
			}
            return posts;
        }
        [Route("getBySearch/{search}")]
        [HttpGet]
        public async Task<ActionResult<PostViewModel[]>> GetBySearch(string search)
        {
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var posts = mapper.Map<PostViewModel[]>(context.Posts.OrderByDescending(x => x.Title.Contains(search)).ThenByDescending(x => x.Text.Contains(search)).ThenByDescending(x => x.CreatedOn).ToArray());
			foreach (var item in posts)
			{
				item.IsOwner = context.Posts.Where(x => x.Id == item.Id && x.AccountId == accountId).FirstOrDefault() != null;
			}
            return posts;
		}
    }
}
