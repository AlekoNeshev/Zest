using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;
using Zest.DBModels;
using Zest.Services.Infrastructure.Interfaces;
using Zest.Services.Infrastructure.Services;

namespace Zest.UnitTests.Services
{
	[TestFixture]
	public class CommunityFollowerServiceTests
	{
		private ZestContext _dbContext;
		private CommunityFollowerService _service;

		[SetUp]
		public void Setup()
		{
			var options = new DbContextOptionsBuilder<ZestContext>()
				.UseInMemoryDatabase(databaseName: "test_database")
				.Options;
			_dbContext = new ZestContext(options);
			_service = new CommunityFollowerService(_dbContext);
		}

		[TearDown]
		public void TearDown()
		{
			_dbContext.Dispose();
		}

		[Test]
		public async Task DoesExistAsync_Returns_True_When_FollowerExists()
		{
			// Arrange
			var accountId = "testUserId";
			var communityId = 1;
			await _dbContext.CommunityFollowers.AddAsync(new CommunityFollower { AccountId = accountId, CommunityId = communityId });
			await _dbContext.SaveChangesAsync();

			// Act
			var result = await _service.DoesExistAsync(accountId, communityId);

			// Assert
			NUnit.Framework.Assert.IsTrue(result);
		}

		[Test]
		public async Task DoesExistAsync_Returns_False_When_FollowerDoesNotExist()
		{
			// Arrange
			var accountId = "testUserId";
			var communityId = 1;

			// Act
			var result = await _service.DoesExistAsync(accountId, communityId);

			// Assert
			NUnit.Framework.Assert.IsFalse(result);
		}

		[Test]
		public async Task AddAsync_AddsFollower_When_FollowerDoesNotExist()
		{
			// Arrange
			var accountId = "testUserId";
			var communityId = 1;

			// Act
			await _service.AddAsync(accountId, communityId);

			// Assert
			var result = await _dbContext.CommunityFollowers.FirstOrDefaultAsync();
			NUnit.Framework.Assert.IsNotNull(result);
			NUnit.Framework.Assert.AreEqual(accountId, result.AccountId);
			NUnit.Framework.Assert.AreEqual(communityId, result.CommunityId);
		}

		[Test]
		public async Task AddAsync_DoesNotAddFollower_When_FollowerAlreadyExists()
		{
			// Arrange
			var accountId = "testUserId";
			var communityId = 1;
			await _dbContext.CommunityFollowers.AddAsync(new CommunityFollower { AccountId = accountId, CommunityId = communityId });
			await _dbContext.SaveChangesAsync();

			// Act
			await _service.AddAsync(accountId, communityId);

			// Assert
			var count = await _dbContext.CommunityFollowers.CountAsync();
			NUnit.Framework.Assert.AreEqual(1, count);
		}

		[Test]
		public async Task DeleteAsync_DeletesFollower_When_FollowerExists()
		{
			// Arrange
			var accountId = "testUserId";
			var communityId = 1;
			await _dbContext.CommunityFollowers.AddAsync(new CommunityFollower { AccountId = accountId, CommunityId = communityId });
			await _dbContext.SaveChangesAsync();

			// Act
			await _service.DeleteAsync(accountId, communityId);

			// Assert
			var result = await _dbContext.CommunityFollowers.FirstOrDefaultAsync();
			NUnit.Framework.Assert.IsNull(result);
		}

		[Test]
		public async Task DeleteAsync_DoesNotDeleteFollower_When_FollowerDoesNotExist()
		{
			// Arrange
			var accountId = "testUserId";
			var communityId = 1;

			// Act
			await _service.DeleteAsync(accountId, communityId);

			// Assert
			var count = await _dbContext.CommunityFollowers.CountAsync();
			NUnit.Framework.Assert.AreEqual(0, count);
		}

	}
}
