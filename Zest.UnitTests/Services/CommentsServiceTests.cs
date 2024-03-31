using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;
using Zest.DBModels;
using Zest.Services.Infrastructure.Services;
using Zest.ViewModels.ViewModels;

namespace Zest.UnitTests.Services
{
	[TestFixture]
	public class CommentsServiceTests
	{
		private CommentsService _commentsService;
		private ZestContext _context;
		private IMapper _mapper;

		[SetUp]
		public void Setup()
		{
			var options = new DbContextOptionsBuilder<ZestContext>()
				.UseInMemoryDatabase(databaseName: "test_database")
				.Options;
			_context = new ZestContext(options);

			_mapper = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<Comment, CommentViewModel>();
				cfg.CreateMap<Like, LikeViewModel>();
			
			}).CreateMapper();

			_commentsService = new CommentsService(_context, _mapper);
		}

		[Test]
		public async Task DoesExist_ReturnsTrue_WhenCommentExists()
		{
			// Arrange
			var comment = new Comment { Id = 1, AccountId="test", Text = "test" };
			await _context.Comments.AddAsync(comment);
			await _context.SaveChangesAsync();

			// Act
			var result = await _commentsService.DoesExist(comment.Id);

			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public async Task DoesExist_ReturnsFalse_WhenCommentDoesNotExist()
		{
			// Arrange
			var commentId = 999;

			// Act
			var result = await _commentsService.DoesExist(commentId);

			// Assert
			Assert.IsFalse(result);
		}

		
		

		[Test]
		public async Task AddAsync_AddsCommentToDatabase()
		{
			// Arrange
			var accountId = "testAccountId";
			var postId = 1;
			var text = "Test Comment";

			// Act
			await _commentsService.AddAsync(accountId, postId, text);
			var result = await _context.Comments.FirstOrDefaultAsync();

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(accountId, result.AccountId);
			Assert.AreEqual(postId, result.PostId);
			Assert.AreEqual(text, result.Text);
		}

		[Test]
		public async Task RemoveAsync_SetsIsDeletedToTrue_WhenCommentExists()
		{
			// Arrange
			var comment = new Comment { Id = 1, AccountId="test", Text="testtext" };
			await _context.Comments.AddAsync(comment);
			await _context.SaveChangesAsync();

			// Act
			await _commentsService.RemoveAsync(comment.Id);
			var result = await _context.Comments.FindAsync(comment.Id);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsTrue(result.IsDeleted);
		}
		
		
	}
	
}

