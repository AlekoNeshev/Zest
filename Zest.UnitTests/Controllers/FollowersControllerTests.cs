using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Zest.Controllers;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

namespace Zest.UnitTests.UnitTests
{
	[TestFixture]
	public class FollowersControllerTests
	{
		private Mock<IFollowerService> _mockFollowerService;
		private Mock<IAccountService> _mockAccountService;
		private FollowersController _controller;

		[SetUp]
		public void Setup()
		{
			_mockFollowerService = new Mock<IFollowerService>();
			_mockAccountService = new Mock<IAccountService>();
			_controller = new FollowersController(_mockFollowerService.Object, _mockAccountService.Object);
		}

		[Test]
		public async Task Find_ShouldReturnFollower_WhenExists()
		{
			// Arrange
			var followerId = "followerId";
			var followedId = "followedId";
			var expectedFollower = new BaseAccountViewModel { Id = followerId, Username = "FollowerUsername", CreatedOn1 = DateTime.Now };
			_mockFollowerService.Setup(x => x.FindAsync(followerId, followedId)).ReturnsAsync(expectedFollower);

			// Act
			var result = await _controller.Find(followerId, followedId);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<ActionResult<BaseAccountViewModel>>(result);
			Assert.AreEqual(expectedFollower, result.Value);
		}

		[Test]
		public async Task Add_ShouldReturnOk_WhenAccountExists()
		{
			// Arrange
			var followedId = "followedId";
			var accountId = "accountId";
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
						new Claim(ClaimTypes.NameIdentifier, accountId)
					}))
				}
			};
			_mockAccountService.Setup(x => x.DoesExistAsync(followedId)).ReturnsAsync(true);

			// Act
			var result = await _controller.Add(followedId);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<OkResult>(result);
		}
		[Test]
		public async Task Delete_ShouldReturnOk_WhenAccountExists()
		{
			// Arrange
			var followedId = "followedId";
			var followerId = "followerId";
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
						new Claim(ClaimTypes.NameIdentifier, followerId)
					}))
				}
			};
			_mockAccountService.Setup(x => x.DoesExistAsync(followedId)).ReturnsAsync(true);

			// Act
			var result = await _controller.Delete(followedId);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<OkResult>(result);
		}

		[Test]
		public async Task FindFriends_ShouldReturnFollowers()
		{
			// Arrange
			var takeCount = 10;
			var skipCount = 0;
			var accountId = "accountId";
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
						new Claim(ClaimTypes.NameIdentifier, accountId)
					}))
				}
			};
			var expectedFollowers = new BaseAccountViewModel[]
			{
				new BaseAccountViewModel { Id = "follower1", Username = "Follower1", CreatedOn1 = DateTime.Now },
				new BaseAccountViewModel { Id = "follower2", Username = "Follower2", CreatedOn1 = DateTime.Now }
			};
			_mockFollowerService.Setup(x => x.FindFriendsAsync(accountId, takeCount, skipCount)).ReturnsAsync(expectedFollowers);

			// Act
			var result = await _controller.FindFriends(takeCount, skipCount);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<ActionResult<BaseAccountViewModel[]>>(result);
			Assert.AreEqual(expectedFollowers, result.Value);
		}

		[Test]
		public async Task GetBySearch_ShouldReturnFollowers()
		{
			// Arrange
			var search = "search";
			var takeCount = 10;
			var skipIds = new string[] { "skip1", "skip2" };
			var accountId = "accountId";
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
						new Claim(ClaimTypes.NameIdentifier, accountId)
					}))
				}
			};
			var expectedFollowers = new BaseAccountViewModel[]
			{
				new BaseAccountViewModel { Id = "follower1", Username = "Follower1", CreatedOn1 = DateTime.Now },
				new BaseAccountViewModel { Id = "follower2", Username = "Follower2", CreatedOn1 = DateTime.Now }
			};
			_mockFollowerService.Setup(x => x.GetBySearchAsync(search, accountId, takeCount, skipIds)).ReturnsAsync(expectedFollowers);

			// Act
			var result = await _controller.GetBySearch(search, takeCount, skipIds);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<ActionResult<BaseAccountViewModel[]>>(result);
			Assert.AreEqual(expectedFollowers, result.Value);
		}
	}
	}
