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

		

		public async Task RemoveLike(int likeId)
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
