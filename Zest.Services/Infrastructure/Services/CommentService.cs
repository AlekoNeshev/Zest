using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;
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
		public async Task<bool> DoesExist(int id)
		{
			var post = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);
			if (post == null)
			{
				return false;
			}
			return true;
		}
		public async Task<CommentViewModel> FindAsync(int id, string accountId)
		{
			
			var comment = _mapper.Map<CommentViewModel>(await _context.Comments.Include(x => x.Account)
	.Include(x => x.Account)
	.Include(x => x.Likes)
		.Include(x => x.Replies).ThenInclude(r => r.Likes)
		.Include(x => x.Replies).ThenInclude(r => r.Account)
			.Include(r => r.Replies).ThenInclude(rr => rr.Replies).ThenInclude(rr => rr.Likes)
			.Include(r => r.Replies).ThenInclude(rr => rr.Replies).ThenInclude(rr => rr.Account)
				.Include(rr => rr.Replies).ThenInclude(x => x.Replies).ThenInclude(x => x.Replies).ThenInclude(x => x.Likes)
				.Include(rr => rr.Replies).ThenInclude(x => x.Replies).ThenInclude(x => x.Replies).ThenInclude(x => x.Account)
					.Include(rrr => rrr.Replies).ThenInclude(x => x.Replies).ThenInclude(x => x.Replies).ThenInclude(rrrr => rrrr.Replies).ThenInclude(l => l.Likes)
					.Include(rrr => rrr.Replies).ThenInclude(x => x.Replies).ThenInclude(x => x.Replies).ThenInclude(rrrr => rrrr.Replies).ThenInclude(l => l.Account).FirstOrDefaultAsync(x => x.Id == id));
			comment.Like = _mapper.Map<LikeViewModel>(await _context.Likes.Where(x => x.AccountId == accountId && x.CommentId == comment.Id).FirstOrDefaultAsync());
			await FindLike(comment.Replies, accountId);
			return comment;
		}
		private async Task<Comment?> FindCommentAsync(int id)
		{
			var comment = await _context.Comments.FindAsync(id);
			return comment;
		}

		public async Task<Comment> AddAsync(string accountId, int postId, string text, int commentId = 0)
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
			return comment.Entity;
		}

		public async Task RemoveAsync(int id)
		{
			var comment = await FindCommentAsync(id);
			if(comment != null)
			{
				comment.IsDeleted = true;
				_context.Update(comment);
				await _context.SaveChangesAsync();
			}		

		}

		public async Task<CommentViewModel[]> GetCommentsByPostIdAsync(int postId, DateTime lastDate, int takeCount, string accountId)
		{


			var comments = _mapper.Map<CommentViewModel[]>(await _context.Comments
	.Where(x => x.PostId == postId && x.CommentId == null && x.CreatedOn < lastDate)
	.Include(x => x.Account)
	.Include(x => x.Likes)
		.Include(x => x.Replies).ThenInclude(r => r.Likes)
		.Include(x => x.Replies).ThenInclude(r => r.Account)
			.Include(r => r.Replies).ThenInclude(rr => rr.Replies).ThenInclude(rr => rr.Likes)
			.Include(r => r.Replies).ThenInclude(rr => rr.Replies).ThenInclude(rr => rr.Account)
				.Include(rr => rr.Replies).ThenInclude(x => x.Replies).ThenInclude(x => x.Replies).ThenInclude(x => x.Likes)
				.Include(rr => rr.Replies).ThenInclude(x => x.Replies).ThenInclude(x => x.Replies).ThenInclude(x => x.Account)
					.Include(rrr => rrr.Replies).ThenInclude(x => x.Replies).ThenInclude(x => x.Replies).ThenInclude(rrrr => rrrr.Replies).ThenInclude(l => l.Likes)
					.Include(rrr => rrr.Replies).ThenInclude(x => x.Replies).ThenInclude(x => x.Replies).ThenInclude(rrrr => rrrr.Replies).ThenInclude(l => l.Account)
	.OrderBy(x => x.CreatedOn)
	.Take(takeCount)
	.ToListAsync()); ;
			foreach (var comment in comments)
			{
				comment.Like = _mapper.Map<LikeViewModel>(await _context.Likes.Where(x => x.AccountId == accountId && x.CommentId == comment.Id).FirstOrDefaultAsync());
				await FindLike(comment.Replies, accountId);
			}
			return comments;
		}
		public async Task<CommentViewModel[]> GetTrendingCommentsAsync(int[] skipIds, int takeCount, string accountId, int postId)
		{


			var comments = await _context.Comments.Where(x => x.PostId == postId).Include(x => x.Account)
			.Include(x => x.Account)
			.Include(x => x.Likes)
				.Include(x => x.Replies).ThenInclude(r => r.Likes)
				.Include(x => x.Replies).ThenInclude(r => r.Account)
					.Include(r => r.Replies).ThenInclude(rr => rr.Replies).ThenInclude(rr => rr.Likes)
					.Include(r => r.Replies).ThenInclude(rr => rr.Replies).ThenInclude(rr => rr.Account)
						.Include(rr => rr.Replies).ThenInclude(x => x.Replies).ThenInclude(x => x.Replies).ThenInclude(x => x.Likes)
						.Include(rr => rr.Replies).ThenInclude(x => x.Replies).ThenInclude(x => x.Replies).ThenInclude(x => x.Account)
							.Include(rrr => rrr.Replies).ThenInclude(x => x.Replies).ThenInclude(x => x.Replies).ThenInclude(rrrr => rrrr.Replies).ThenInclude(l => l.Likes)
							.Include(rrr => rrr.Replies).ThenInclude(x => x.Replies).ThenInclude(x => x.Replies).ThenInclude(rrrr => rrrr.Replies).ThenInclude(l => l.Account)
			.ToArrayAsync();

			var likeWeight = 1.0;
			var commentWeight = 0.5;
			var decayFactor = 0.9;

			var scores = comments.Select(p => new { Comment = p, Score = CalculateScore(p, likeWeight, commentWeight, decayFactor) });

			var filteredScores = scores.Where(s => !skipIds.Contains(s.Comment.Id)).OrderByDescending(s => s.Score);

			var trendingComments = _mapper.Map<CommentViewModel[]>(filteredScores.Take(takeCount).Select(x => x.Comment).ToArray());
			foreach (var item in trendingComments)
			{
				item.Like = _mapper.Map<LikeViewModel>(await _context.Likes.Where(x => x.AccountId == accountId && x.CommentId == item.Id).FirstOrDefaultAsync());
			}
			return trendingComments;
		}
		private double CalculateScore(Comment comment, double likeWeight, double commentWeight, double decayFactor)
		{
			var now = DateTime.UtcNow;
			var likeScore = comment.Likes.Sum(l => Math.Pow(decayFactor, (now - l.CreatedOn).TotalHours)) * likeWeight;
			var commentScore = comment.Replies.Sum(c => Math.Pow(decayFactor, (now - c.CreatedOn).TotalHours)) * commentWeight;

			return likeScore + commentScore;
		}
		private async Task FindLike(IEnumerable<CommentViewModel> commentViewModels, string accountId)
		{
			foreach (var commentViewModel in commentViewModels)
			{
				if (commentViewModel == null)
				{
					return;
				}
				commentViewModel.Like = _mapper.Map<LikeViewModel>(await _context.Likes.Where(x => x.AccountId == accountId && x.CommentId == commentViewModel.Id).FirstOrDefaultAsync());
				await FindLike(commentViewModel.Replies, accountId);
			}

		}
	}
}
