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
		Task<FollowerViewModel> FindAsync(string followerId, string followedId);
		Task AddAsync(string followerId, string followedId);
		Task DeleteAsync(string followerId, string followedId);
		Task<FollowerViewModel[]> FindFriendsAsync(string accountId);
	}
}
