using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;

namespace Zest.Services.Infrastructure.Interfaces
{
	public interface ICommentsService
	{
		Task<Comment> FindAsync(int id);
		Task<EntityEntry<Comment>> AddAsync(string accountId, int postId, string text, int commentId);
		Task RemoveAsync(Comment comment);
		Task<Comment[]> GetCommentsByPostIdAsync(int postId, DateTime lastDate, int takeCount);
	}
}
