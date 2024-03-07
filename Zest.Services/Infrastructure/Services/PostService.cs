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
	public class PostService : IPostService
	{
		private readonly ZestContext _context;

		public PostService(ZestContext context)
		{
			_context = context;
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

		public async Task<List<Post>> GetByDateAsync(DateTime lastDate, int minimumSkipCount, int takeCount)
		{
			return await _context.Posts
				.OrderByDescending(p => p.CreatedOn)
				.Skip(minimumSkipCount)
				.Where(x => x.CreatedOn < lastDate)
				.Take(takeCount)
				.ToListAsync();
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
