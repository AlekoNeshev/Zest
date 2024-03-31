using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;
using Zest.DBModels;
using Zest.Services.Infrastructure.Services;

namespace Zest.UnitTests.Services
{
	[TestFixture]
	public class LikeServiceTests
	{
		private LikeService _likeService;
		private ZestContext _context;

		[SetUp]
		public void Setup()
		{
			var options = new DbContextOptionsBuilder<ZestContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;
			_context = new ZestContext(options);
			_context.Database.EnsureCreated();

			_likeService = new LikeService(_context);
		}

		[TearDown]
		public void TearDown()
		{
			_context.Dispose();
		}

		[Test]
		public async Task AddLikeToPostAsync_CreatesLikeForPost()
		{
			// Arrange
			var accountId = "testAccountId";
			var postId = 1;
			var value = true;

			// Act
			await _likeService.AddLikeToPostAsync(accountId, postId, value);
			var like = await _context.Likes.FirstOrDefaultAsync();

			// Assert
			Assert.IsNotNull(like);
			Assert.AreEqual(accountId, like.AccountId);
			Assert.AreEqual(postId, like.PostId);
			Assert.AreEqual(value, like.Value);
		}

		[Test]
		public async Task AddLikeToCommentAsync_CreatesLikeForComment()
		{
			// Arrange
			var accountId = "testAccountId";
			var commentId = 1;
			var value = true;

			// Act
			await _likeService.AddLikeToCommentAsync(accountId, commentId, value);
			var like = await _context.Likes.FirstOrDefaultAsync();

			// Assert
			Assert.IsNotNull(like);
			Assert.AreEqual(accountId, like.AccountId);
			Assert.AreEqual(commentId, like.CommentId);
			Assert.AreEqual(value, like.Value);
		}

		[Test]
		public async Task RemoveLikeAsync_RemovesLike()
		{
			// Arrange
			var like = new Like { AccountId = "testAccountId", PostId = 1, Value = true };
			await _context.Likes.AddAsync(like);
			await _context.SaveChangesAsync();

			// Act
			await _likeService.RemoveLikeAsync(like.Id);
			var removedLike = await _context.Likes.FindAsync(like.Id);

			// Assert
			Assert.IsNull(removedLike);
		}
	}
}
