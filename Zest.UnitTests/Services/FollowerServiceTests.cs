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
	public class FollowerServiceTests
	{
		private FollowerService _followerService;
		private ZestContext _context;
		private IMapper _mapper;

		[SetUp]
		public void Setup()
		{
			var mapperConfig = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<Follower, BaseAccountViewModel>()
					.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.FollowedId))
					.ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Followed.Username));
				
			});
			_mapper = mapperConfig.CreateMapper();

			var options = new DbContextOptionsBuilder<ZestContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;
			_context = new ZestContext(options);
			_context.Database.EnsureCreated();

			_followerService = new FollowerService(_context, _mapper);
		}

		[TearDown]
		public void TearDown()
		{
			_context.Dispose();
		}

		[Test]
		public async Task FindAsync_ReturnsBaseAccountViewModel_WhenFollowerExists()
		{
			// Arrange
			var followerId = "followerId";
			var followedId = "followedId";
			var follower = new Account { Id = followerId, Username = "Follower", Email = "test" };
			var followed = new Account { Id = followedId, Username = "Followed" , Email = "test" };
			await _context.Followers.AddAsync(new Follower { FollowerId = followerId, FollowedId = followedId });
			await _context.Accounts.AddRangeAsync(follower, followed);
			await _context.SaveChangesAsync();

			// Act
			var result = await _followerService.FindAsync(followerId, followedId);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(followedId, result.Id);
			Assert.AreEqual("Followed", result.Username);
		}
		[Test]
		public async Task AddAsync_CreatesFollower()
		{
			// Arrange
			var followerId = "followerId";
			var followedId = "followedId";

			// Act
			await _followerService.AddAsync(followerId, followedId);
			var addedFollower = await _context.Followers.FirstOrDefaultAsync(x => x.FollowerId == followerId && x.FollowedId == followedId);

			// Assert
			Assert.IsNotNull(addedFollower);
		}

		

		
	}
	}
