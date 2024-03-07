using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;

namespace Zest.Services.Infrastructure.Interfaces
{
	public interface ICommunityService
	{
		Task<Community> GetCommunityByIdAsync(int id);
		Task<Community[]> GetAllCommunitiesAsync(string accountId);
		Task<int> AddCommunityAsync(string creatorId, string name, string discription);
	}

}
