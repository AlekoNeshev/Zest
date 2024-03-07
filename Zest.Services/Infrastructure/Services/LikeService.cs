﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;
using Zest.DBModels;
using Zest.Services.Infrastructure.Interfaces;

namespace Zest.Services.Infrastructure.Services
{
	public class LikeService : ILikeService
	{
		private readonly ZestContext _context;

		public LikeService(ZestContext context)
		{
			_context = context;
		}

		public async Task AddLikeToPost(string accountId, int postId, bool value)
		{
			_context.Likes.Add(new Like
			{
				AccountId = accountId,
				PostId = postId,
				Value = value,
				CreatedOn = DateTime.Now
			});

			await _context.SaveChangesAsync();
		}

		public async Task AddLikeToComment(string accountId, int commentId, bool value)
		{
			_context.Likes.Add(new Like
			{
				AccountId = accountId,
				CommentId = commentId,
				Value = value,
				CreatedOn = DateTime.Now
			});

			await _context.SaveChangesAsync();
		}

		public async Task RemoveLikeFromPost(string accountId, int postId)
		{
			var like = _context.Likes.FirstOrDefault(l => l.AccountId == accountId && l.PostId == postId);

			if (like != null)
			{
				_context.Likes.Remove(like);
				await _context.SaveChangesAsync();
			}
		}

		public async Task RemoveLikeFromComment(string accountId, int commentId)
		{
			var like = _context.Likes.FirstOrDefault(l => l.AccountId == accountId && l.CommentId == commentId);

			if (like != null)
			{
				_context.Likes.Remove(like);
				await _context.SaveChangesAsync();
			}
		}
	}
}
