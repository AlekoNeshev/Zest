using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Zest.Controllers;
using Zest.Services.Hubs;
using Zest.Services.Infrastructure.Interfaces;

namespace Zest.UnitTests.UnitTests
{
	[TestFixture]
	public class LikesControllerTests
	{
		private Mock<ILikeService> _mockLikeService;
		private Mock<IHubContext<LikesHub>> _mockHubContext;
		private Mock<IPostService> _mockPostService;
		private Mock<ICommentsService> _mockCommentsService;
		private LikesController _controller;

		[SetUp]
		public void Setup()
		{
			_mockLikeService = new Mock<ILikeService>();
			_mockHubContext = new Mock<IHubContext<LikesHub>>();
			_mockPostService = new Mock<IPostService>();
			_mockCommentsService = new Mock<ICommentsService>();
			_controller = new LikesController(_mockLikeService.Object, _mockHubContext.Object, _mockPostService.Object, _mockCommentsService.Object);
		}

		[Test]
		public async Task Add_ShouldReturnOk_WhenPostExists()
		{
			// Arrange
			var postId = 1;
			var commentId = 0;
			var value = true;
			var accountId = "accountId";
			var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
			var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = user }
			};
			_mockPostService.Setup(x => x.DoesExist(postId)).ReturnsAsync(true);
			_mockHubContext.Setup(x => x.Clients.Group(It.IsAny<string>())).Returns(Mock.Of<IClientProxy>());
			// Act
			var result = await _controller.Add(postId, commentId, value);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<OkResult>(result);
		}

		[Test]
		public async Task Remove_ShouldReturnOk_WhenPostExists()
		{
			// Arrange
			var likeId = 1;
			var postId = 1;
			var commentId = 0;
			var accountId = "accountId";
			var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
			var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = user }
			};
			_mockPostService.Setup(x => x.DoesExist(postId)).ReturnsAsync(true);
			_mockHubContext.Setup(x => x.Clients.Group(It.IsAny<string>())).Returns(Mock.Of<IClientProxy>());
			// Act
			var result = await _controller.Remove(likeId, postId, commentId);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<OkResult>(result);
		}
		[Test]
		public async Task Add_ShouldReturnOk_WhenCommentIdIsNotZeroAndCommentExists()
		{
			// Arrange
			var postId = 1;
			var commentId = 1;
			var value = true;
			var accountId = "accountId";
			var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
			var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = user }
			};
			_mockPostService.Setup(x => x.DoesExist(postId)).ReturnsAsync(true);
			_mockCommentsService.Setup(x => x.DoesExist(commentId)).ReturnsAsync(true);
			_mockHubContext.Setup(x => x.Clients.Group(It.IsAny<string>())).Returns(Mock.Of<IClientProxy>());
			// Act
			var result = await _controller.Add(postId, commentId, value);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<OkResult>(result);
		}

		[Test]
		public async Task Remove_ShouldReturnOk_WhenCommentIdIsNotZeroAndCommentExists()
		{
			// Arrange
			var likeId = 1;
			var postId = 1;
			var commentId = 1;
			var accountId = "accountId";
			var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
			var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = user }
			};
			_mockPostService.Setup(x => x.DoesExist(postId)).ReturnsAsync(true);
			_mockCommentsService.Setup(x => x.DoesExist(commentId)).ReturnsAsync(true);
			_mockHubContext.Setup(x => x.Clients.Group(It.IsAny<string>())).Returns(Mock.Of<IClientProxy>());
			// Act
			var result = await _controller.Remove(likeId, postId, commentId);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<OkResult>(result);
		}
	}
}
