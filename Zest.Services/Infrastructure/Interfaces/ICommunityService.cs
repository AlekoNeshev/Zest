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
		Task<CommunityViewModel> GetCommunityByIdAsync(int id);
		Task<CommunityViewModel[]> GetAllCommunitiesAsync(string accountId, int skipCount, int takeCount);
		Task<int> AddCommunityAsync(string creatorId, string name, string discription);
		Task<CommunityViewModel[]> GetCommunitiesByAccount(string accountId, int takeCount, int skipCount);
		Task<CommunityViewModel[]> GetTrendingCommunities(int[] skipIds, int takeCount);
		Task<CommunityViewModel[]> GetBySearchAsync(string search, int takeCount, int[]? skipIds);
	}

}
