using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

namespace Zest.Services.Infrastructure.Services
{
	public class FollowerService : IFollowerService
	{
		private readonly ZestContext _context;
		private readonly IMapper _mapper;

		public FollowerService(ZestContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<FollowerViewModel> FindAsync(string followerId, string followedId)
		{
			return _mapper.Map<FollowerViewModel>(await _context.Followers.Include(x=>x.Followed).Include(x=>x.FollowerNavigation).FirstOrDefaultAsync(x => x.FollowerId == followerId && x.FollowedId == followedId));
		}

		public async Task AddAsync(string followerId, string followedId)
		{
			await _context.Followers.AddAsync(new Follower { FollowerId = followerId, FollowedId = followedId, CreatedOn = DateTime.Now });
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(string followerId, string followedId)
		{
			var follower = await _context.Followers.Include(x => x.Followed).Include(x => x.FollowerNavigation).FirstOrDefaultAsync(x => x.FollowerId == followerId && x.FollowedId == followedId);
			_context.Followers.Remove(follower);
			await _context.SaveChangesAsync();
		}

		public async Task<FollowerViewModel[]> FindFriendsAsync(string accountId)
		{
			var followers = await _context.Followers.Where(x => x.FollowedId == accountId).Include(x => x.Followed).Include(x => x.FollowerNavigation).ToListAsync();
			var following = await _context.Followers.Where(x => x.FollowerId == accountId).Include(x => x.Followed).Include(x => x.FollowerNavigation).ToListAsync();

			var friends = followers
				.Where(follower => following.Any(follow => follow.FollowedId == follower.FollowerId))
				.ToList();

			return _mapper.Map<FollowerViewModel[]>(friends);
		}
	}
}
