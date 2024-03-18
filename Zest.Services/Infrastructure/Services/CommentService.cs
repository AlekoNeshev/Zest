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
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NodaTime;
using NodaTime.Extensions;

namespace Zest.Services.Infrastructure.Services
{
	public class CommentsService : ICommentsService
	{
		private readonly ZestContext _context;
		private readonly IMapper _mapper;

		public CommentsService(ZestContext context, IMapper mapper)
		{
			_context = context;
			this._mapper = mapper;
		}

		public async Task<Comment> FindAsync(int id)
		{
			var comment = await _context.Comments.FindAsync(id);
			return comment;
		}

		public async Task<EntityEntry<Comment>> AddAsync(string accountId,int postId, string text, int commentId = 0)
		{
			EntityEntry<Comment> comment;
			
			if (commentId == 0)
			{
				comment = await _context.AddAsync(new Comment { AccountId = accountId, PostId = postId, Text = text, CreatedOn = DateTime.Now });			
			}
			else 
			{
				 comment = await _context.AddAsync(new Comment { AccountId = accountId, PostId = postId, CommentId = commentId, Text = text, CreatedOn = DateTime.Now });			
			}
			
			await _context.SaveChangesAsync();
			return comment;
		}

		public async Task RemoveAsync(Comment comment)
		{
			comment.IsDeleted = true;
			_context.Update(comment);
			await _context.SaveChangesAsync();
		}

		public async Task<Comment[]> GetCommentsByPostIdAsync(int postId, DateTime lastDate, int takeCount)
		{
			
			var coms = await _context.Comments.Where(x=>x.CreatedOn > lastDate).ToArrayAsync();
			var comments = await _context.Comments
				
				.Where(x => x.PostId == postId && x.CommentId == null && x.CreatedOn < lastDate)
				.OrderBy(x=>x.CreatedOn)
				.Take(takeCount)
				.ToArrayAsync();
			return comments;
		}
	}
}
