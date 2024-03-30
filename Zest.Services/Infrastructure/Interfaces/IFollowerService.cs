using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;
using Zest.ViewModels.ViewModels;

namespace Zest.Services.Infrastructure.Interfaces
{
	public interface IFollowerService
	{
		Task<BaseAccountViewModel> FindAsync(string followerId, string followedId);
		Task AddAsync(string followerId, string followedId);
		Task DeleteAsync(string followerId, string followedId);
		Task<BaseAccountViewModel[]> FindFriendsAsync(string accountId, int takeCount, int skipCount);
		Task<BaseAccountViewModel[]> GetBySearchAsync(string search, string accountId, int takeCount, string[]? skipIds);
	}
}
