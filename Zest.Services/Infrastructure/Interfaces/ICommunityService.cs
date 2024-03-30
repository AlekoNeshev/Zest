using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;
using Zest.ViewModels.ViewModels;

namespace Zest.Services.Infrastructure.Interfaces
{
	public interface ICommunityService
	{
		Task<CommunityViewModel> GetCommunityByIdAsync(int id, string accountId);
		Task<CommunityViewModel[]> GetAllCommunitiesAsync(string accountId, int skipCount, int takeCount);
		Task<int> AddCommunityAsync(string creatorId, string name, string discription);
		Task<CommunityViewModel[]> GetCommunitiesByAccount(string accountId, int takeCount, int skipCount);
		Task<CommunityViewModel[]> GetTrendingCommunitiesAsync(int[] skipIds, int takeCount, string accountId);
		Task<CommunityViewModel[]> GetBySearchAsync(string search, string accountId,int takeCount, int[]? skipIds);
		Task DeleteCommunityAsync(int communityId);
		Task<bool> DoesExistAsync(int id);
	}

}
