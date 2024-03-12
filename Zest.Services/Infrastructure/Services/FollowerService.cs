using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;
using Zest.DBModels;
using Zest.Services.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Zest.Services.Infrastructure.Services
{
	public class FollowerService : IFollowerService
	{
		private readonly ZestContext _context;

		public FollowerService(ZestContext context)
		{
			_context = context;
		}

		public async Task<Follower> FindAsync(string followerId, string followedId)
		{
			return await _context.Followers.FirstOrDefaultAsync(x => x.FollowerId == followerId && x.FollowedId == followedId);
		}

		public async Task AddAsync(string followerId, string followedId)
		{
			await _context.Followers.AddAsync(new Follower { FollowerId = followerId, FollowedId = followedId, CreatedOn = DateTime.Now });
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(string followerId, string followedId)
		{
			var follower = await FindAsync(followerId, followedId);
			_context.Followers.Remove(follower);
			await _context.SaveChangesAsync();
		}

		public async Task<List<Follower>> FindFriendsAsync(string accountId)
		{
			var followers = await _context.Followers.Where(x => x.FollowedId == accountId).ToListAsync();
			var following = await _context.Followers.Where(x => x.FollowerId == accountId).ToListAsync();

			var friends = followers
				.Where(follower => following.Any(follow => follow.FollowedId == follower.FollowerId))
				.ToList();

			return friends;
		}
	}
}
