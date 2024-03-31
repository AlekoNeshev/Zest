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

namespace Zest.UnitTests.UnitTests
{
	[TestFixture]
	public class CommunityFollowersControllerTests
	{
		private Mock<ICommunityFollowerService> _communityFollowerServiceMock;
		private CommunityFollowersController _controller;

		[SetUp]
		public void Setup()
		{
			_communityFollowerServiceMock = new Mock<ICommunityFollowerService>();
			_controller = new CommunityFollowersController(_communityFollowerServiceMock.Object);
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
						new Claim(ClaimTypes.NameIdentifier, "testUserId")
					}))
				}
			};
		}

		[Test]
		public async Task DoesExist_Returns_True_When_CommunityFollowersExist()
		{
			// Arrange
			int communityId = 1;
			_communityFollowerServiceMock.Setup(x => x.DoesExistAsync("testUserId", communityId)).ReturnsAsync(true);

			// Act
			var result = await _controller.DoesExist(communityId);

			// Assert
			Assert.IsTrue(result.Value);
		}

		[Test]
		public async Task Add_Returns_OkResult_When_FollowerAddedSuccessfully()
		{
			// Arrange
			int communityId = 1;
			_communityFollowerServiceMock.Setup(x => x.DoesExistAsync("testUserId", communityId)).ReturnsAsync(false);

			// Act
			var result = await _controller.Add(communityId);

			// Assert
			Assert.IsInstanceOf<OkResult>(result);
		}

		[Test]
		public async Task Add_Returns_BadRequest_When_FollowerAlreadyExists()
		{
			// Arrange
			int communityId = 1;
			_communityFollowerServiceMock.Setup(x => x.DoesExistAsync("testUserId", communityId)).ReturnsAsync(true);

			// Act
			var result = await _controller.Add(communityId);

			// Assert
			Assert.IsInstanceOf<BadRequestObjectResult>(result);
		}

		[Test]
		public async Task Delete_Returns_OkResult_When_FollowerDeletedSuccessfully()
		{
			// Arrange
			int communityId = 1;
			_communityFollowerServiceMock.Setup(x => x.DoesExistAsync("testUserId", communityId)).ReturnsAsync(true);

			// Act
			var result = await _controller.Delete(communityId);

			// Assert
			Assert.IsInstanceOf<OkResult>(result);
		}

		[Test]
		public async Task Delete_Returns_BadRequest_When_FollowerDoesNotExist()
		{
			// Arrange
			int communityId = 1;
			_communityFollowerServiceMock.Setup(x => x.DoesExistAsync("testUserId", communityId)).ReturnsAsync(false);

			// Act
			var result = await _controller.Delete(communityId);

			// Assert
			Assert.IsInstanceOf<BadRequestObjectResult>(result);
		}
	}
}
