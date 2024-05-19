using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

namespace Zest.Services.Infrastructure.Services
{
	public class CommunityService : ICommunityService
	{
		private readonly ZestContext _context;
		private readonly IMapper _mapper;

		public CommunityService(ZestContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<bool> DoesExistAsync(int id)
		{
			var post = await _context.Communities.FirstOrDefaultAsync(x => x.Id == id);
			if (post == null)
			{
				return false;
			}
			return true;
		}
		public async Task<CommunityViewModel> GetCommunityByIdAsync(int id, string accountId)
		{
			var community = _mapper.Map<CommunityViewModel>(await _context.Communities.Include(x => x.Creator).FirstOrDefaultAsync(x => x.Id == id));
			community.IsSubscribed = await _context.CommunityFollowers.Where(x => x.CommunityId == id && x.AccountId == accountId).FirstOrDefaultAsync() != null;
			return community;

		}

		public async Task<CommunityViewModel[]> GetAllCommunitiesAsync(string accountId, int skipCount, int takeCount)
		{
			var communities = _mapper.Map<CommunityViewModel[]>(await _context.Communities.Include(x => x.Creator).Skip(skipCount).Take(takeCount).ToArrayAsync());
			foreach (var community in communities) 
			{
				community.IsSubscribed = await _context.CommunityFollowers.Where(x => x.CommunityId == community.Id && x.AccountId == accountId).FirstOrDefaultAsync() != null;
			}
			return communities;
		}

		public async Task<int> AddCommunityAsync(string creatorId, string name, string discription)
		{
			var community = new Community
			{
				Name = name,
				Information = discription,
				CreatorId = creatorId,
				CreatedOn = DateTime.UtcNow,
			};
			await _context.AddAsync(community);
			
			await _context.SaveChangesAsync();
			var communityModerator = new CommunityModerator
			{
				CommunityId = community.Id,
				AccountId = creatorId,
				IsApproved = true,
				CreatedOn = DateTime.UtcNow
			};
			await _context.AddAsync(communityModerator);
			await _context.SaveChangesAsync();
			return community.Id;
		}
		public async Task<CommunityViewModel[]> GetCommunitiesByAccount(string accountId, int takeCount, int skipCount)
		{
			var communities = _mapper.Map<CommunityViewModel[]>(_context.CommunityFollowers.Where(x => x.AccountId==accountId).Skip(skipCount).Take(takeCount).Include(x => x.Community).Select(x => x.Community).ToArray());
			foreach (var community in communities)
			{
				community.IsSubscribed = await _context.CommunityFollowers.Where(x => x.CommunityId == community.Id && x.AccountId == accountId).FirstOrDefaultAsync() != null;
			}
			return communities;
		}
		public async Task<CommunityViewModel[]> GetTrendingCommunitiesAsync(int[] skipIds, int takeCount, string accountId)
		{	
			var filteredCommunities = _context.Communities.Include(x=>x.Posts).ThenInclude(x=>x.Comments).Include(x=>x.Creator).Where(c => !skipIds.Contains(c.Id)).ToArray();

			var communityScores = new List<(Community Community, int Score)>();

			foreach (var community in filteredCommunities)
			{
				var (userCount, postCount, likeCount, commentCount) = CalculateCommunityStats(community);
				var score = CalculateCommunityScore(userCount, postCount, likeCount, commentCount);
				communityScores.Add((Community: community, Score: score));
			}
			var communities = _mapper.Map<CommunityViewModel[]>(communityScores.OrderByDescending(c => c.Score).Take(takeCount).Select(c => c.Community).ToArray());
			foreach (var community in communities)
			{
				community.IsSubscribed = await _context.CommunityFollowers.Where(x => x.CommunityId == community.Id && x.AccountId == accountId).FirstOrDefaultAsync() != null;
			}
			return communities;
		}
		private (int userCount, int postCount, int likeCount, int commentCount) CalculateCommunityStats(Community community)
		{
			var cutoffDate = DateTime.UtcNow - TimeSpan.FromHours(72);
			var userCount = _context.CommunityFollowers.Count(cf => cf.CommunityId == community.Id);
			var recentPosts = community.Posts.Where(x => x.CreatedOn >= cutoffDate).ToArray();
			var postCount = recentPosts.Count();
			var likeCount = recentPosts.Sum(p => p.Likes.Count + p.Comments.Sum(c => c.Likes.Count));
			var commentCount = community.Posts.Sum(p => p.Comments.Count());

			return (userCount, postCount, likeCount, commentCount);
		}
		private int CalculateCommunityScore(int userCount, int postCount, int likeCount, int commentCount)
		{
			const int userWeight = 3;
			const int postWeight = 2;
			const int likeWeight = 1;
			const int commentWeight = 1;

			return userWeight * userCount + postWeight * postCount + likeWeight * likeCount + commentWeight * commentCount;
		}
		public async Task<CommunityViewModel[]> GetBySearchAsync(string search, string accountId,int takeCount, int[] skipIds)
		{
			var communities = _mapper.Map<CommunityViewModel[]>(await _context.Communities.Where(p => !skipIds.Contains(p.Id))
				.OrderByDescending(x => x.Name.Contains(search))
				.ThenByDescending(x => x.Information.Contains(search))
				.ThenByDescending(x => x.CreatedOn)
				.Take(takeCount)
				.ToArrayAsync());

			foreach (var community in communities)
			{
				community.IsSubscribed = await _context.CommunityFollowers.Where(x => x.CommunityId == community.Id && x.AccountId == accountId).FirstOrDefaultAsync() != null;
			}

			return communities;
		}
		public async Task DeleteCommunityAsync(int communityId)
		{
			var community = await _context.Communities.FindAsync(communityId);
			_context.Communities.Remove(community);
			await _context.SaveChangesAsync();
		}
	}
}
