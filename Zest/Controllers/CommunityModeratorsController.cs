using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityModeratorsController : Controller
    {
        private ZestContext context;
		private IMapper mapper;
		public CommunityModeratorsController(ZestContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        [Authorize]
        [Route("isModerator/{accountId}/{communityId}")]
        [HttpGet]
        public async Task<ActionResult<bool>> Find(string accountId, int communityId)
        {
            var cm = context.CommunityModerators.FirstOrDefault(x=>x.AccountId == accountId && x.CommunityId==communityId&&x.IsApproved==true);
            if (cm == null)
                return false;
            return true;
        }
        [Authorize]
        [Route("add/{accountId}/{communityId}")]
        [HttpPost]
        public async Task<ActionResult> Add(string accountId, int communityId)
        {
			
			context.Add(new CommunityModerator { AccountId = accountId, CommunityId = communityId, IsApproved=false,CreatedOn = DateTime.Now });
            context.SaveChanges();
            return Ok();
        }
        [Authorize]
        [Route("getModerators/{communityId}")]
        [HttpGet]
        public async Task<UserViewModel[]> GetModeratorsByCommunity(int communityId)
        {
            return mapper.Map<UserViewModel[]>(context.CommunityModerators.Where(x=>x.CommunityId == communityId && x.IsApproved == true).Select(x=>x.Account)).ToArray();
        }
		[Authorize]
		[Route("getCandidates/{communityId}")]
		[HttpGet]
		public async Task<UserViewModel[]> GetModeratorCandidatesByCommunity(int communityId)
		{
			return mapper.Map<UserViewModel[]>(context.CommunityModerators.Where(x => x.CommunityId == communityId && x.IsApproved == false).Select(x => x.Account)).ToArray();
		}
		[Authorize]
        [Route("approveCandidate/{accountId}/{communityId}")]
        [HttpPost]
        public async Task<ActionResult> ApproveCandidate(string accountId, int communityId)
        {
            var candidate = context.CommunityModerators.Where(x=>x.AccountId == accountId && x.CommunityId == communityId).FirstOrDefault();
            if(candidate != null)
            {
				candidate.IsApproved = true;
                context.SaveChanges();
                return Ok();
			}
            return BadRequest();

        }
		[Authorize]
		[Route("removeModerator/{accountId}/{communityId}")]
		[HttpPost]
		public async Task<ActionResult> RemoveModerator(string accountId, int communityId)
		{
			var candidate = context.CommunityModerators.Where(x => x.AccountId == accountId && x.CommunityId == communityId).FirstOrDefault();
			if (candidate != null)
			{
				context.CommunityModerators.Remove(candidate);
				context.SaveChanges();
				return Ok();
			}
			return BadRequest();

		}
	}
}
