using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Services.Infrastructure.Interfaces;
using Zest.Services.Infrastructure.Services;
using Zest.ViewModels.ViewModels;

namespace Zest.UnitTests.Services
{
	[TestFixture]
	public class CommunityServiceTests
	{
		private CommunityService _communityService;
		private ZestContext _context;
		private IMapper _mapper;

		[SetUp]
		public void Setup()
		{
			
			var mapperConfig = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<Community, CommunityViewModel>();
				
			});
			_mapper = mapperConfig.CreateMapper();

			
			var options = new DbContextOptionsBuilder<ZestContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;
			_context = new ZestContext(options);
			_context.Database.EnsureCreated();

			
			_communityService = new CommunityService(_context, _mapper);
		}

		[TearDown]
		public void TearDown()
		{
			_context.Dispose(); // Dispose the DbContext after each test
		}

		[Test]
		public async Task DoesExistAsync_ReturnsTrue_WhenCommunityExists()
		{
			// Arrange
			var community = new Community { Id = 1, CreatorId="test", Information="test", Name="test" };
			await _context.Communities.AddAsync(community);
			await _context.SaveChangesAsync();

			// Act
			var result = await _communityService.DoesExistAsync(community.Id);

			// Assert
			NUnit.Framework.Assert.IsTrue(result);
		}

		[Test]
		public async Task DoesExistAsync_ReturnsFalse_WhenCommunityDoesNotExist()
		{
			// Arrange
			var communityId = 999;

			// Act
			var result = await _communityService.DoesExistAsync(communityId);

			// Assert
			NUnit.Framework.Assert.IsFalse(result);
		}
		
		

		[Test]
		public async Task AddCommunityAsync_CreatesCommunityWithModerator()
		{
			// Arrange
			var creatorId = "testCreatorId";
			var name = "Test Community";
			var description = "Test Description";

			// Act
			var resultId = await _communityService.AddCommunityAsync(creatorId, name, description);
			var addedCommunity = await _context.Communities.FindAsync(resultId);
			var moderator = await _context.CommunityModerators.FirstOrDefaultAsync(x => x.CommunityId == resultId && x.AccountId == creatorId);

			// Assert
			NUnit.Framework.Assert.IsNotNull(addedCommunity);
			NUnit.Framework.Assert.AreEqual(name, addedCommunity.Name);
			NUnit.Framework.Assert.AreEqual(description, addedCommunity.Information);
			NUnit.Framework.Assert.IsNotNull(moderator);
			NUnit.Framework.Assert.AreEqual(creatorId, moderator.AccountId);
			NUnit.Framework.Assert.IsTrue(moderator.IsApproved);
		}

		[Test]
		public async Task GetCommunitiesByAccount_ReturnsCorrectNumberOfCommunitiesWithSubscriptionStatus()
		{
			// Arrange
			var accountId = "testAccountId";
			var communities = new[]
			{
				new Community { Id = 1, CreatorId="test", Information="test", Name="test" },
				new Community { Id = 2 , CreatorId="test", Information="test", Name="test"},
				new Community { Id = 3 , CreatorId="test", Information="test", Name="test"}
			};
			await _context.Communities.AddRangeAsync(communities);
			await _context.SaveChangesAsync();
			foreach (var community in communities)
			{
				await _context.CommunityFollowers.AddAsync(new CommunityFollower { CommunityId = community.Id, AccountId = accountId });
			}
			await _context.SaveChangesAsync();

			// Act
			var result = await _communityService.GetCommunitiesByAccount(accountId, takeCount: 10, skipCount: 0);

			// Assert
			NUnit.Framework.Assert.IsNotNull(result);
			NUnit.Framework.Assert.AreEqual(3, result.Length);
			foreach (var community in result)
			{
				NUnit.Framework.Assert.IsTrue(community.IsSubscribed);
			}
		}

	

		[Test]
		public async Task GetBySearchAsync_ReturnsCorrectNumberOfCommunitiesWithSubscriptionStatus()
		{
			// Arrange
			var accountId = "testAccountId";
			var search = "Test";
			var skipIds = new int[0];
			var communities = new[]
			{
				new Community { Id = 1,  CreatorId="test", Information="test", Name="test" },
				new Community { Id = 2,  CreatorId="test", Information="test", Name="test" },
				new Community { Id = 3,  CreatorId="test", Information="test", Name="test" }
			};
			await _context.Communities.AddRangeAsync(communities);
			await _context.SaveChangesAsync();

			// Act
			var result = await _communityService.GetBySearchAsync(search, accountId, takeCount: 10, skipIds);

			// Assert
			NUnit.Framework.Assert.IsNotNull(result);
			NUnit.Framework.Assert.AreEqual(3, result.Length);
			foreach (var community in result)
			{
				NUnit.Framework.Assert.IsFalse(community.IsSubscribed);
			}
		}
	
	}
}
