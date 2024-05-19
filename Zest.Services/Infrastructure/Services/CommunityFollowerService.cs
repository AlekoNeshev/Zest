using Microsoft.EntityFrameworkCore;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Services.Infrastructure.Interfaces;

namespace Zest.Services.Infrastructure.Services
{
	public class CommunityFollowerService : ICommunityFollowerService
	{
		private readonly ZestContext _context;

		public CommunityFollowerService(ZestContext context)
		{
			_context = context;
		}

		public async Task<bool> DoesExistAsync(string accountId, int communityId)
		{
			return  await _context.CommunityFollowers.AnyAsync(x => x.AccountId == accountId && x.CommunityId == communityId);
		}

		public async Task AddAsync(string accountId, int communityId)
		{
			if (!await DoesExistAsync(accountId, communityId))
			{
				_context.Add(new CommunityFollower { AccountId = accountId, CommunityId = communityId, CreatedOn = DateTime.UtcNow });
				await _context.SaveChangesAsync();
			}
		}

		public async Task DeleteAsync(string accountId, int communityId)
		{
			var communityFollower = await _context.CommunityFollowers.FirstOrDefaultAsync(x => x.AccountId == accountId && x.CommunityId == communityId);

			if (communityFollower != null)
			{
				_context.Remove(communityFollower);
				await _context.SaveChangesAsync();
			}
		}
		
	}
}
