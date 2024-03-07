using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;

namespace Zest.Services.Infrastructure.Interfaces
{
	public interface ICommunityModeratorService
	{
		Task<bool> IsModeratorAsync(string accountId, int communityId);
		Task AddModeratorAsync(string accountId, int communityId);
		Task<List<Account>> GetModeratorsByCommunityAsync(int communityId);
		Task<List<Account>> GetModeratorCandidatesByCommunityAsync(int communityId);
		Task ApproveCandidateAsync(string accountId, int communityId);
		Task RemoveModeratorAsync(string accountId, int communityId);
	}
}
