using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
    [ApiController]
    public class CommunityController : ControllerBase
    {
		private readonly ICommunityService _communityService;
		private readonly ICommunityFollowerService _communityFollowerService;
		private readonly IMapper _mapper;

		public CommunityController(ICommunityService communityService, ICommunityFollowerService communityFollowerService,IMapper mapper)
		{
			_communityService = communityService;
			_communityFollowerService = communityFollowerService;
			_mapper = mapper;
		}

		[Route("{id}")]
		[HttpGet]
		public async Task<ActionResult<CommunityViewModel>> Find(int id)
		{
			var community = await _communityService.GetCommunityByIdAsync(id);
			return _mapper.Map<CommunityViewModel>(community);
		}

		[Route("getAll")]
		[HttpGet]
		public async Task<ActionResult<CommunityViewModel[]>> GetAll()
		{
			var accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var communities = await _communityService.GetAllCommunitiesAsync(accountId);
		 var communityViewModels = _mapper.Map<CommunityViewModel[]>(communities);
			foreach (var item in communityViewModels)
			{
				item.IsSubscribed = await  _communityFollowerService.DoesExistAsync(accountId, item.Id);
			}
			return communityViewModels;
		}

		[Route("add/{name}")]
		[HttpPost]
		public async Task<ActionResult> Add(string name, [FromBody] string discription)
		{
			var creatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			
			var communityId = await _communityService.AddCommunityAsync(creatorId, name, discription);
			return Ok(communityId);
		}
		[Route("GetByAccountId/{accountId}")]
		[HttpGet]
		public async Task<ActionResult<CommunityViewModel[]>> GetCommunitiesByAccount(string accountId)
		{
			var communities = await _communityService.GetCommunitiesByAccount(accountId);
			return communities;
		}
	}
}
