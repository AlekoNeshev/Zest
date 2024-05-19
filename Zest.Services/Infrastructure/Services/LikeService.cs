using Zest.DBModels;
using Zest.DBModels.Models;
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

		public async Task AddLikeToPostAsync(string accountId, int postId, bool value)
		{
			await _context.Likes.AddAsync(new Like
			{
				AccountId = accountId,
				PostId = postId,
				Value = value,
				CreatedOn = DateTime.UtcNow
			});

			await _context.SaveChangesAsync();
		}

		public async Task AddLikeToCommentAsync(string accountId, int commentId, bool value)
		{
			await _context.Likes.AddAsync(new Like
			{
				AccountId = accountId,
				CommentId = commentId,
				Value = value,
				CreatedOn = DateTime.UtcNow
			});

			await _context.SaveChangesAsync();
		}

		

		public async Task RemoveLikeAsync(int likeId)
		{
			var like = await _context.Likes.FindAsync(likeId);
			
			if (like != null)
			{				
				_context.Likes.Remove(like);
				await _context.SaveChangesAsync();
						
			}
		
		}
	}
}
