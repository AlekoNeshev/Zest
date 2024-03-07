using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zest.Services.Infrastructure.Interfaces
{
	public interface ILikeService
	{
		Task AddLikeToPost(string accountId, int postId, bool value);
		Task AddLikeToComment(string accountId, int commentId, bool value);
		Task RemoveLikeFromPost(string accountId, int postId);
		Task RemoveLikeFromComment(string accountId, int commentId);
	}
}
