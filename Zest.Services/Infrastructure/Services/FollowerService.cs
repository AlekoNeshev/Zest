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

		public async Task<BaseAccountViewModel> FindAsync(string followerId, string followedId)
		{
			return _mapper.Map<BaseAccountViewModel>(await _context.Followers.Include(x=>x.Followed).Include(x=>x.FollowerNavigation).FirstOrDefaultAsync(x => x.FollowerId == followerId && x.FollowedId == followedId));
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

		public async Task<BaseAccountViewModel[]> FindFriendsAsync(string accountId, int takeCount, int skipCount)
		{
			var followers = await _context.Followers.Where(x => x.FollowedId == accountId).Include(x => x.Followed).Include(x => x.FollowerNavigation).ToListAsync();
			var following = await _context.Followers.Where(x => x.FollowerId == accountId).Include(x => x.Followed).Include(x => x.FollowerNavigation).ToListAsync();

			var friends = followers
				.Where(follower => following.Any(follow => follow.FollowedId == follower.FollowerId)).Skip(skipCount).Take(takeCount)
				.ToList();
			var friendsViewModels = _mapper.Map<List<BaseAccountViewModel>>(friends);
			if(skipCount == 0) 
			{
				friendsViewModels.AddRange(_mapper.Map<BaseAccountViewModel[]>(await _context.Accounts.Where(x => x.IsAdmin == true && x.Id != accountId).ToArrayAsync()));
			}
			
			return friendsViewModels.GroupBy(x => x.Id).Select(group => group.First()).ToArray();
		}
		public async Task<BaseAccountViewModel[]> GetBySearchAsync(string search, string accountId, int takeCount, string[] skipIds)
		{
			var accounts = await FindFriendsAsync(accountId, _context.Followers.Count(), 0);
				accounts = accounts.Where(p => !skipIds.Contains(p.Id)).OrderByDescending(x => x.Username.Contains(search)).Take(takeCount).ToArray();

			return accounts;
		}
	}
}
