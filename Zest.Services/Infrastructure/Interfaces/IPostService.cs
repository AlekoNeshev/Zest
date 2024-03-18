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
		Task<Post> FindAsync(int id);
		Task<Post> AddAsync(string title, string text, string accountId, int communityId);
		Task RemoveAsync(int id);
		Task<List<Post>> GetByDateAsync(DateTime lastDate, int minimumSkipCount, int takeCount);
		Task<List<Post>> GetByCommunityAsync(int communityId);
		Task<List<Post>> GetBySearchAsync(string search);
		Task<bool> IsOwnerAsync(int postId, string accountId);
		Task<List<PostViewModel>> GetFollowedPostsAsync(int[] skipIds, int takeCount, string accountId);
		Task<List<PostViewModel>> GetTrending(int[] skipIds, int takeCount, int communityId = 0);
	}
}
