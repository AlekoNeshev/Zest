using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

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
		public async Task<bool> DoesExist(int id)
		{
			var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == id);
			if(post == null)
			{
				return false;
			}
			return true;
		}
		public async Task<PostViewModel?> FindAsync(int id, string accountId)
		{
			var post =  _mapper.Map<PostViewModel>(await _context.Posts.Include(x=>x.Likes).Include(x=>x.Account).Include(x=>x.PostResources).FirstOrDefaultAsync(x=>x.Id == id));
			if(post == null)
			{
				return null;
			}
			post.Like = _mapper.Map<LikeViewModel>(await _context.Likes.Where(x => x.AccountId == accountId && x.PostId == post.Id).FirstOrDefaultAsync()); ;
			return post;
		}

		public async Task<Post> AddAsync(string title, string text, string accountId, int communityId)
		{
			var post = new Post
			{
				Title = title,
				Text = text,
				AccountId = accountId,
				CommunityId = communityId,
				CreatedOn = DateTime.UtcNow,
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

		public async Task<PostViewModel[]> GetByDateAsync(string accountId, DateTime lastDate, int communityId, int takeCount)
		{

			if (communityId != 0)
			{
				var posts = await _context.Posts
			   .Where(x => x.CommunityId == communityId && x.CreatedOn < lastDate && x.IsDeleted != true)
			   .Include(x => x.Likes)
				.Include(x => x.Account)
				.Include(x => x.Community)
				.Include(x => x.PostResources)
			   .OrderByDescending(p => p.CreatedOn)
			   .Take(takeCount)
			   .ToArrayAsync();
				var pvm = _mapper.Map<PostViewModel[]>(posts);
				foreach (var item in pvm)
				{
					item.Like = _mapper.Map<LikeViewModel>(await _context.Likes.Where(x => x.AccountId == accountId && x.PostId == item.Id).FirstOrDefaultAsync());
				}
				return pvm;
			}
			else
			{
				var posts = await _context.Posts
			   .Where(x => x.CreatedOn < lastDate && x.IsDeleted != true)
			   .Include(x => x.Likes)
				.Include(x => x.Account)
				.Include(x => x.Community)
				.Include(x => x.PostResources)
			   .OrderByDescending(p => p.CreatedOn)
			   .Take(takeCount)
			   .ToArrayAsync();
				var pvm = _mapper.Map<PostViewModel[]>(posts);
				foreach (var item in pvm)
				{
					item.Like = _mapper.Map<LikeViewModel>(await _context.Likes.Where(x => x.AccountId == accountId && x.PostId == item.Id).FirstOrDefaultAsync());
				}
				return pvm;
			}
		}
		public async Task<PostViewModel[]> GetTrendingAsync(int[] skipIds, int takeCount, string accountId, int communityId = 0)
		{
			
			var cutoffDate = DateTime.UtcNow - TimeSpan.FromHours(72);
			var posts = await _context.Posts.Where(x => x.CreatedOn >= cutoffDate).Include(x => x.Likes)
				.Include(x => x.Account).Include(x=>x.Community).Include(x => x.PostResources).ToArrayAsync();
			if (communityId > 0)
			{
				posts = posts.Where(x => x.CommunityId == communityId).ToArray();
			}
			var likeWeight = 1.0;
			var commentWeight = 0.5;
			var decayFactor = 0.9;

			var scores = posts.Select(p => new { Post = p, Score = CalculateScore(p, likeWeight, commentWeight, decayFactor) });

			var filteredScores = scores.Where(s => !skipIds.Contains(s.Post.Id)).OrderByDescending(s => s.Score);

			var trendingPosts = _mapper.Map<PostViewModel[]>(filteredScores.Take(takeCount).Select(x => x.Post).ToArray());
			foreach (var item in trendingPosts)
			{
				item.Like = _mapper.Map<LikeViewModel>(await _context.Likes.Where(x => x.AccountId == accountId && x.PostId == item.Id ).FirstOrDefaultAsync());
			}
			return trendingPosts;
		}
		public async Task<PostViewModel[]> GetFollowedPostsAsync(int[] skipIds, int takeCount, string accountId)
		{

			var followedUserIds = await _context.Followers
				.Where(f => f.FollowerId == accountId)
				.Select(f => f.FollowedId)
				.ToArrayAsync();
			var followedCommunityIds = await _context.CommunityFollowers
				.Where(cf => cf.AccountId == accountId)
				.Select(cf => cf.CommunityId)
				.ToArrayAsync();
			var posts = _mapper.Map<PostViewModel[]>(await _context.Posts
				.Where(p =>
					followedUserIds.Contains(p.AccountId) ||
					followedCommunityIds.Contains(p.CommunityId))
				.Where(p => !skipIds.Contains(p.Id))
				.Include(x=>x.Likes)
				.Include(x=>x.Account)
				.Include(x => x.Community)
				.Include(x => x.PostResources)
				.OrderByDescending(p => p.CreatedOn)
				.Take(takeCount)
				.ToArrayAsync());
			foreach (var item in posts)
			{
				item.Like = _mapper.Map<LikeViewModel>(await _context.Likes.Where(x => x.AccountId == accountId&& x.PostId == item.Id).FirstOrDefaultAsync());
			}
			return posts;
		}
		private double CalculateScore(Post post, double likeWeight, double commentWeight, double decayFactor)
		{
			var now = DateTime.UtcNow;
			var likeScore = post.Likes.Sum(l => Math.Pow(decayFactor, (now - l.CreatedOn).TotalHours)) * likeWeight;
			var commentScore = post.Comments.Sum(c => Math.Pow(decayFactor, (now - c.CreatedOn).TotalHours)) * commentWeight;

			return likeScore + commentScore;
		}
		public async Task<PostViewModel[]> GetByCommunityAsync(int communityId)
		{
			return _mapper.Map<PostViewModel[]>(await _context.Posts
				.Where(x => x.CommunityId == communityId)
				.Include(x => x.Likes)
				.Include(x => x.Account)
				.Include(x => x.Community)
				.Include(x => x.PostResources)
				.OrderByDescending(x => x.CreatedOn)
				.ToArrayAsync());
		}

		public async Task<PostViewModel[]> GetBySearchAsync(string search, string accountId, int takeCount, int communityId, int[]? skipIds)
		{
			if (communityId != 0)
			{
				var posts = _mapper.Map<PostViewModel[]>(await _context.Posts.Where(p => !skipIds.Contains(p.Id) && p.CommunityId == communityId)
				.OrderByDescending(x => x.Title.Contains(search))
				.ThenByDescending(x => x.Text.Contains(search))
				.ThenByDescending(x => x.CreatedOn)
				.Include(x => x.Likes)
				.Include(x => x.Account)
				.Include(x => x.Community)
				.Include(x => x.PostResources)
				.Take(takeCount)
				.ToArrayAsync());

				foreach (var item in posts)
				{
					item.Like = _mapper.Map<LikeViewModel>(await _context.Likes.Where(x => x.AccountId == accountId&& x.PostId == item.Id).FirstOrDefaultAsync());					
				}
				return posts;
			}
			else
			{

				var posts = _mapper.Map<PostViewModel[]>(await _context.Posts.Where(p => !skipIds.Contains(p.Id))
					.OrderByDescending(x => x.Title.Contains(search))
					.ThenByDescending(x => x.Text.Contains(search))
					.ThenByDescending(x => x.CreatedOn)
					.Include(x => x.Likes)
					.Include(x => x.Account)
					.Include(x => x.Community)
					.Include(x => x.PostResources)
					.Take(takeCount)
					.ToArrayAsync());

				foreach (var item in posts)
				{
					item.Like = _mapper.Map<LikeViewModel>(await _context.Likes.Where(x => x.AccountId == accountId&& x.PostId == item.Id).FirstOrDefaultAsync());				
				}
				return posts;
			}
				
		}

		public async Task<bool> IsOwnerAsync(int postId, string accountId)
		{
			return await _context.Posts.AnyAsync(x => x.Id == postId && x.AccountId == accountId);
		}
	}
}
