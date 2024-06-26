﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;
using Zest.ViewModels.ViewModels;

namespace Zest.Services.Infrastructure.Interfaces
{
	public interface ICommentsService
	{
		Task<CommentViewModel> FindAsync(int id, string accountId);
		Task<Comment> AddAsync(string accountId, int postId, string text, int commentId = 0);
		Task RemoveAsync(int id);
		Task<CommentViewModel[]> GetCommentsByPostIdAsync(int postId, DateTime lastDate, int takeCount, string accountId);
		Task<CommentViewModel[]> GetTrendingCommentsAsync(int[] skipIds, int takeCount, string accountId, int postId);
		Task<bool> DoesExist(int id);

	}
}
