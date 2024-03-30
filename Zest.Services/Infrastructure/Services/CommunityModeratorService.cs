using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

namespace Zest.Services.Infrastructure.Services
{
	public class CommunityModeratorService : ICommunityModeratorService
	{
		private readonly ZestContext context;
		private readonly IMapper _mapper;
		public CommunityModeratorService(ZestContext context, IMapper mapper)
		{
			this.context = context;
			this._mapper = mapper;
		}
		public async Task<bool> IsModeratorAsync(string accountId, int communityId)
		{
			var cm = await context.CommunityModerators.FirstOrDefaultAsync(x => x.AccountId == accountId && x.CommunityId == communityId && x.IsApproved == true);
			return cm != null;
		}
		public async Task<bool> IsModeratorCandidateAsync(string accountId, int communityId)
		{
			var cm = await context.CommunityModerators.FirstOrDefaultAsync(x => x.AccountId == accountId && x.CommunityId == communityId && x.IsApproved == false);
			return cm != null;
		}
		public async Task AddModeratorAsync(string accountId, int communityId)
		{
			context.Add(new CommunityModerator { AccountId = accountId, CommunityId = communityId, IsApproved = false, CreatedOn = DateTime.Now });
			await context.SaveChangesAsync();
		}

		public async Task<UserViewModel[]> GetModeratorsByCommunityAsync(int communityId)
		{
			return _mapper.Map<UserViewModel[]>(await context.CommunityModerators.Where(x => x.CommunityId == communityId && x.IsApproved == true).Include(x=>x.Account).Select(x => x.Account).ToListAsync());
		}

		public async Task<UserViewModel[]> GetModeratorCandidatesByCommunityAsync(int communityId)
		{
			return _mapper.Map<UserViewModel[]>(await context.CommunityModerators.Where(x => x.CommunityId == communityId && x.IsApproved == false).Include(x => x.Account).Select(x => x.Account).ToListAsync());
		}

		public async Task ApproveCandidateAsync(string accountId, int communityId)
		{
			var candidate = await context.CommunityModerators.Where(x => x.AccountId == accountId && x.CommunityId == communityId).FirstOrDefaultAsync();
			if (candidate != null)
			{
				candidate.IsApproved = true;
				await context.SaveChangesAsync();
			}
		}

		public async Task RemoveModeratorAsync(string accountId, int communityId)
		{
			var candidate = await context.CommunityModerators.Where(x => x.AccountId == accountId && x.CommunityId == communityId).FirstOrDefaultAsync();
			if (candidate != null)
			{
				context.CommunityModerators.Remove(candidate);
				await context.SaveChangesAsync();
			}
		}
	}
}
