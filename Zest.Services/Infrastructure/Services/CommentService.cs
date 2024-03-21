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

		public async Task<CommentViewModel> FindAsync(int id, string accountId)
		{
			var comment = _mapper.Map<CommentViewModel>(await _context.Comments.FindAsync(id));
			comment.Like = _mapper.Map<LikeViewModel>(await _context.Likes.Where(x => x.AccountId == accountId && x.CommentId == comment.Id).FirstOrDefaultAsync());
			return comment;
		}
		private async Task<Comment> FindCommentAsync(int id)
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

		public async Task RemoveAsync(int id)
		{
			var comment  = await FindCommentAsync(id);
			comment.IsDeleted = true;
			_context.Update(comment);
			await _context.SaveChangesAsync();
		}

		public async Task<CommentViewModel[]> GetCommentsByPostIdAsync(int postId, DateTime lastDate, int takeCount, string accountId)
		{
			
			
			var comments = _mapper.Map<CommentViewModel[]>(await _context.Comments
				
				.Where(x => x.PostId == postId && x.CommentId == null && x.CreatedOn < lastDate)
				.OrderBy(x=>x.CreatedOn)
				.Take(takeCount)
				.ToArrayAsync());
			foreach (var comment in comments)
			{
				comment.Like = _mapper.Map<LikeViewModel>(await _context.Likes.Where(x => x.AccountId == accountId && x.CommentId == comment.Id).FirstOrDefaultAsync());
			}
			return comments;
		}
	}
}
