using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;
using Zest.DBModels;
using Zest.Services.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Zest.ViewModels.ViewModels;
using AutoMapper;

namespace Zest.Services.Infrastructure.Services
{
	public class PostService : IPostService
	{
		private readonly ZestContext _context;
		private readonly IMapper _mapper;
		public PostService(ZestContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<Post> FindAsync(int id)
		{
			return await _context.Posts.FindAsync(id);
		}

		public async Task<Post> AddAsync(string title, string text, string accountId, int communityId)
		{
			var post = new Post
			{
				Title = title,
				Text = text,
				AccountId = accountId,
				CommunityId = communityId,
				CreatedOn = DateTime.Now,
			};

			await _context.AddAsync(post);
			await _context.SaveChangesAsync();

			return post;
		}

		public async Task RemoveAsync(int id)
		{
			var post = await _context.Posts.FindAsync(id);

			if (post == null)
			{
				return;
			}

			post.IsDeleted = true;
			_context.Posts.Update(post);
			await _context.SaveChangesAsync();
		}

		public async Task<List<Post>> GetByDateAsync(DateTime lastDate, int communityId, int takeCount)
		{
			
			if(communityId != 0)
			{
				return await _context.Posts
				.Where(x => x.CommunityId == communityId && x.CreatedOn < lastDate)
				.OrderByDescending(p => p.CreatedOn)
				.Take(takeCount)
				.ToListAsync();
			}
			return await _context.Posts
				.Where(x => x.CreatedOn < lastDate)
				.OrderByDescending(p => p.CreatedOn)
				.Take(takeCount)
				.ToListAsync();
		}
		public async Task<List<PostViewModel>> GetTrending(int[] skipIds, int takeCount, int communityId = 0)
		{
			var cutoffDate = DateTime.UtcNow - TimeSpan.FromHours(72);
			var posts = _context.Posts.Where(x => x.CreatedOn >= cutoffDate).ToList();
			if(communityId > 0)
			{
				posts = posts.Where(x=>x.CommunityId == communityId).ToList();
			}
			var likeWeight = 1.0;
			var commentWeight = 0.5;
			var decayFactor = 0.9;

			var scores = posts.Select(p => new { Post = p, Score = CalculateScore(p, likeWeight, commentWeight, decayFactor) });

			var filteredScores = scores.Where(s => !skipIds.Contains(s.Post.Id)).OrderByDescending(s => s.Score);

			var trendingPosts = _mapper.Map<List<PostViewModel>>(filteredScores.Take(takeCount).Select(x=>x.Post).ToList());

			return trendingPosts;
		}
		public async Task<List<PostViewModel>> GetFollowedPostsAsync(int[] skipIds, int takeCount, string accountId)
		{
			var followedUserIds = _context.Followers
	.Where(f => f.FollowerId == accountId)
	.Select(f => f.FollowedId)
	.ToList();
			var followedCommunityIds = _context.CommunityFollowers
	.Where(cf => cf.AccountId == accountId)
	.Select(cf => cf.CommunityId)
	.ToList();
			var posts = _mapper.Map<List<PostViewModel>>(_context.Posts
	.Where(p =>
		followedUserIds.Contains(p.AccountId) ||
		followedCommunityIds.Contains(p.CommunityId))
	.Where(p => !skipIds.Contains(p.Id))
	.OrderByDescending(p => p.CreatedOn)
	.Take(takeCount)
	.ToList());
			return posts;
		}
		private double CalculateScore(Post post, double likeWeight, double commentWeight, double decayFactor)
		{
			var now = DateTime.UtcNow;
			var likeScore = post.Likes.Sum(l => Math.Pow(decayFactor, (now - l.CreatedOn).TotalHours)) * likeWeight;
			var commentScore = post.Comments.Sum(c => Math.Pow(decayFactor, (now - c.CreatedOn).TotalHours)) * commentWeight;

			return likeScore + commentScore;
		}
		public async Task<List<Post>> GetByCommunityAsync(int communityId)
		{
			return await _context.Posts
				.Where(x => x.CommunityId == communityId)
				.OrderByDescending(x => x.CreatedOn)
				.ToListAsync();
		}

		public async Task<List<Post>> GetBySearchAsync(string search)
		{
			return await _context.Posts
				.OrderByDescending(x => x.Title.Contains(search))
				.ThenByDescending(x => x.Text.Contains(search))
				.ThenByDescending(x => x.CreatedOn)
				.ToListAsync();
		}

		public async Task<bool> IsOwnerAsync(int postId, string accountId)
		{
			return await _context.Posts.AnyAsync(x => x.Id == postId && x.AccountId == accountId);
		}
	}
}
