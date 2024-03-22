﻿using AutoMapper;
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

		public async Task<Community> GetCommunityByIdAsync(int id)
		{
			return await _context.Communities.FindAsync(id);
		}

		public async Task<Community[]> GetAllCommunitiesAsync(string accountId)
		{
			var communities = _context.Communities.ToArray();
			
			return communities;
		}

		public async Task<int> AddCommunityAsync(string creatorId, string name, string discription)
		{
			var community = new Community
			{
				Name = name,
				Information = discription,
				CreatorId = creatorId,
				CreatedOn = DateTime.Now,
			};
			await _context.AddAsync(community);
			
			await _context.SaveChangesAsync();
			var communityModerator = new CommunityModerator
			{
				CommunityId = community.Id,
				AccountId = creatorId,
				IsApproved = true,
				CreatedOn = DateTime.Now
			};
			await _context.AddAsync(communityModerator);
			await _context.SaveChangesAsync();
			return community.Id;
		}
		public async Task<CommunityViewModel[]> GetCommunitiesByAccount(string accountId)
		{
			return _mapper.Map<CommunityViewModel[]>(_context.CommunityFollowers.Where(x => x.AccountId==accountId).Select(x => x.Community).ToArray());
		}
		public async Task<CommunityViewModel[]> GetTrendingCommunities(int[] skipIds, int takeCount)
		{
			

			var filteredCommunities = _context.Communities.Where(c => !skipIds.Contains(c.Id)).ToArray();


			var communityScores = new List<(Community Community, int Score)>();

			foreach (var community in filteredCommunities)
			{
				var (userCount, postCount, likeCount, commentCount) = CalculateCommunityStats(community);
				var score = CalculateCommunityScore(userCount, postCount, likeCount, commentCount);
				communityScores.Add((Community: community, Score: score));
			}

			return _mapper.Map<CommunityViewModel[]>(communityScores.OrderByDescending(c => c.Score).Take(takeCount).Select(c => c.Community).ToArray());
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

	}
}