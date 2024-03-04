using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
    [ApiController]
    public class CommunityController : ControllerBase
    {
        private ZestContext context;
        private IMapper mapper;
        public CommunityController(ZestContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult<CommunityViewModel>> Find(int id)
        {
            return mapper.Map<CommunityViewModel>(context.Communities.Where(x => x.Id == id).FirstOrDefault());
        }
        [Route("getAll")]
        [HttpGet]
        public async Task<ActionResult<CommunityViewModel[]>> GetAll()
        {
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			CommunityViewModel[] communityViewModels = mapper.Map<CommunityViewModel[]>(context.Communities.ToArray());
            foreach (var item in communityViewModels)
            {
                item.IsSubscribed = context.CommunityFollowers.Where(x=>x.CommunityId == item.Id && x.AccountId == accountId).FirstOrDefault() != null;
            }
			return communityViewModels;
        }
        [Route("add/{name}")]
        [HttpPost]
        public async Task<ActionResult> Add(string name, [FromBody] string discription)
        {
			var user = User.Claims;
			var creatorId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var community =  context.Add(new Community
            {
                Name = name,
                Information = discription,
                CreatorId = creatorId,
                CreatedOn = DateTime.Now,
            });
            context.SaveChanges();
			var communityId = community.Property<int>("Id").CurrentValue;
			return Ok(communityId);
        }
    }
}
