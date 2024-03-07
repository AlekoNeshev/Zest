using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;

namespace Zest.Services.Infrastructure.Interfaces
{
	public interface IFollowerService
	{
		Task<Follower> FindAsync(string followerId, string followedId);
		Task AddAsync(string followerId, string followedId);
		Task DeleteAsync(string followerId, string followedId);
		Task<List<Follower>> FindFriendsAsync(string accountId);
	}
}
