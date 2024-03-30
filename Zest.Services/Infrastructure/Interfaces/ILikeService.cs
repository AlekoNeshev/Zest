using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zest.Services.Infrastructure.Interfaces
{
	public interface ILikeService
	{
		Task AddLikeToPostAsync(string accountId, int postId, bool value);
		Task AddLikeToCommentAsync(string accountId, int commentId, bool value);
		Task RemoveLikeAsync(int likeId);
	}
}
