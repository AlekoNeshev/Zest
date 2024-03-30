using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zest.Services.Infrastructure.Interfaces
{
	public interface ICommunityFollowerService
	{
		Task<bool> DoesExistAsync(string accountId, int communityId);
		Task AddAsync(string accountId, int communityId);
		Task DeleteAsync(string accountId, int communityId);
	}
}
