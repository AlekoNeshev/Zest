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
	public class CommunityModeratorServiceTests
	{
		private CommunityModeratorService _communityModeratorService;
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
				cfg.CreateMap<CommunityModerator, UserViewModel>()
					.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Account.Id))
					.ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Account.Username));
				
			}).CreateMapper();

			_communityModeratorService = new CommunityModeratorService(_context, _mapper);
		}

		[Test]
		public async Task IsModeratorAsync_ReturnsTrue_WhenModeratorExists()
		{
			// Arrange
			var accountId = "testAccountId";
			var communityId = 1;
			await _context.CommunityModerators.AddAsync(new CommunityModerator { AccountId = accountId, CommunityId = communityId, IsApproved = true });
			await _context.SaveChangesAsync();

			// Act
			var result = await _communityModeratorService.IsModeratorAsync(accountId, communityId);

			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public async Task IsModeratorAsync_ReturnsFalse_WhenModeratorDoesNotExist()
		{
			// Arrange
			var accountId = "testAccountId";
			var communityId = 1;

			// Act
			var result = await _communityModeratorService.IsModeratorAsync(accountId, communityId);

			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public async Task IsModeratorCandidateAsync_ReturnsTrue_WhenModeratorCandidateExists()
		{
			// Arrange
			var accountId = "testAccountId";
			var communityId = 1;
			await _context.CommunityModerators.AddAsync(new CommunityModerator { AccountId = accountId, CommunityId = communityId, IsApproved = false });
			await _context.SaveChangesAsync();

			// Act
			var result = await _communityModeratorService.IsModeratorCandidateAsync(accountId, communityId);

			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public async Task IsModeratorCandidateAsync_ReturnsFalse_WhenModeratorCandidateDoesNotExist()
		{
			// Arrange
			var accountId = "testAccountId";
			var communityId = 1;

			// Act
			var result = await _communityModeratorService.IsModeratorCandidateAsync(accountId, communityId);

			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public async Task AddModeratorAsync_AddsModeratorToDatabase()
		{
			// Arrange
			var accountId = "testAccountId";
			var communityId = 1;

			// Act
			await _communityModeratorService.AddModeratorAsync(accountId, communityId);
			var result = await _context.CommunityModerators.FirstOrDefaultAsync();

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(accountId, result.AccountId);
			Assert.AreEqual(communityId, result.CommunityId);
			Assert.IsFalse(result.IsApproved);
		}

		

		

		[Test]
		public async Task ApproveCandidateAsync_ApprovesModeratorCandidate()
		{
			// Arrange
			var accountId = "testAccountId";
			var communityId = 1;
			await _context.CommunityModerators.AddAsync(new CommunityModerator { AccountId = accountId, CommunityId = communityId, IsApproved = false });
			await _context.SaveChangesAsync();

			// Act
			await _communityModeratorService.ApproveCandidateAsync(accountId, communityId);
			var result = await _context.CommunityModerators.FirstOrDefaultAsync();

			// Assert
			Assert.IsNotNull(result);
			Assert.IsTrue(result.IsApproved);
		}

		[Test]
		public async Task RemoveModeratorAsync_RemovesModerator()
		{
			// Arrange
			var accountId = "testAccountId";
			var communityId = 1;
			await _context.CommunityModerators.AddAsync(new CommunityModerator { AccountId = accountId, CommunityId = communityId, IsApproved = true });
			await _context.SaveChangesAsync();

			// Act
			await _communityModeratorService.RemoveModeratorAsync(accountId, communityId);
			var result = await _context.CommunityModerators.FirstOrDefaultAsync();

			// Assert
			Assert.IsNull(result);
		}
	}
}
