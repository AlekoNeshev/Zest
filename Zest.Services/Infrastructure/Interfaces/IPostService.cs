using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;
using Zest.ViewModels.ViewModels;

namespace Zest.Services.Infrastructure.Interfaces
{
	public interface IPostService
	{
		Task<PostViewModel> FindAsync(int id, string accountId);
		Task<Post> AddAsync(string title, string text, string accountId, int communityId);
		Task RemoveAsync(int id);
		Task<PostViewModel[]> GetByDateAsync(string accountId, DateTime lastDate, int communityId, int takeCount);
		Task<PostViewModel[]> GetByCommunityAsync(int communityId);
		Task<PostViewModel[]> GetBySearchAsync(string search, string accountId);
		Task<bool> IsOwnerAsync(int postId, string accountId);
		Task<PostViewModel[]> GetFollowedPostsAsync(int[] skipIds, int takeCount, string accountId);
		Task<PostViewModel[]> GetTrending(int[] skipIds, int takeCount, string accountId,int communityId = 0);
	}
}
