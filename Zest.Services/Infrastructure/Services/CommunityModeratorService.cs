using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Services.Infrastructure.Interfaces;

namespace Zest.Services.Infrastructure.Services
{
	public class CommunityModeratorService : ICommunityModeratorService
	{
		private readonly ZestContext context;

		public CommunityModeratorService(ZestContext context)
		{
			this.context = context;
		}
		public async Task<bool> IsModeratorAsync(string accountId, int communityId)
		{
			var cm = context.CommunityModerators.FirstOrDefault(x => x.AccountId == accountId && x.CommunityId == communityId && x.IsApproved == true);
			return cm != null;
		}

		public async Task AddModeratorAsync(string accountId, int communityId)
		{
			context.Add(new CommunityModerator { AccountId = accountId, CommunityId = communityId, IsApproved = false, CreatedOn = DateTime.Now });
			await context.SaveChangesAsync();
		}

		public async Task<List<Account>> GetModeratorsByCommunityAsync(int communityId)
		{
			return await context.CommunityModerators.Where(x => x.CommunityId == communityId && x.IsApproved == true).Select(x => x.Account).ToListAsync();
		}

		public async Task<List<Account>> GetModeratorCandidatesByCommunityAsync(int communityId)
		{
			return await context.CommunityModerators.Where(x => x.CommunityId == communityId && x.IsApproved == false).Select(x => x.Account).ToListAsync();
		}

		public async Task ApproveCandidateAsync(string accountId, int communityId)
		{
			var candidate = context.CommunityModerators.Where(x => x.AccountId == accountId && x.CommunityId == communityId).FirstOrDefault();
			if (candidate != null)
			{
				candidate.IsApproved = true;
				await context.SaveChangesAsync();
			}
		}

		public async Task RemoveModeratorAsync(string accountId, int communityId)
		{
			var candidate = context.CommunityModerators.Where(x => x.AccountId == accountId && x.CommunityId == communityId).FirstOrDefault();
			if (candidate != null)
			{
				context.CommunityModerators.Remove(candidate);
				await context.SaveChangesAsync();
			}
		}
	}
}
