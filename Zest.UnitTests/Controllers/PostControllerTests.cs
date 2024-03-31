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
using Zest.DBModels.Models;
using Zest.Services.Hubs;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

namespace Zest.UnitTests.UnitTests
{
	[TestFixture]
	public class PostControllerTests
	{
		private Mock<IPostService> _mockPostService;
		private Mock<IHubContext<DeleteHub>> _mockHubContext;
		private Mock<ICommunityService> _mockCommunityService;
		private PostController _controller;

		[SetUp]
		public void Setup()
		{
			_mockPostService = new Mock<IPostService>();
			_mockHubContext = new Mock<IHubContext<DeleteHub>>();
			_mockCommunityService = new Mock<ICommunityService>();
			_controller = new PostController(_mockPostService.Object, _mockHubContext.Object, _mockCommunityService.Object);
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
		public async Task Find_Returns_Post_When_Found()
		{
			// Arrange
			var postId = 1;
			var expectedPost = new PostViewModel { Id = postId };
			_mockPostService.Setup(x => x.FindAsync(postId, "testUserId")).ReturnsAsync(expectedPost);

			// Act
			var result = await _controller.Find(postId);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<ActionResult<PostViewModel>>(result);
			Assert.IsNotNull(result.Value);
			Assert.AreEqual(expectedPost, result.Value);
		}

		[Test]
		public async Task Find_Returns_BadRequest_When_Post_Not_Found()
		{
			// Arrange
			var postId = 1;
			_mockPostService.Setup(x => x.FindAsync(postId, "testUserId")).ReturnsAsync((PostViewModel)null);

			// Act
			var result = await _controller.Find(postId);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
			var badRequestResult = (BadRequestObjectResult)result.Result;
			Assert.AreEqual("Post does not exist", badRequestResult.Value);
		}
		[Test]
		public async Task Add_Returns_OkResult_When_Community_Exists()
		{
			// Arrange
			var title = "Test Title";
			var text = "Test Text";
			var communityId = 1;
			var userId = "user123";
			var postId = 1;
			_mockCommunityService.Setup(x => x.DoesExistAsync(communityId)).ReturnsAsync(true);
			_mockPostService.Setup(x => x.AddAsync(title, text, "testUserId", communityId)).ReturnsAsync(new Post { Id = postId });

			// Act
			var result = await _controller.Add(title, text, communityId);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<OkObjectResult>(result);
			var okResult = (OkObjectResult)result;
			Assert.AreEqual(postId, okResult.Value);
		}

		[Test]
		public async Task Add_Returns_BadRequest_When_Community_Does_Not_Exist()
		{
			// Arrange
			var title = "Test Title";
			var text = "Test Text";
			var communityId = 1;
			_mockCommunityService.Setup(x => x.DoesExistAsync(communityId)).ReturnsAsync(false);

			// Act
			var result = await _controller.Add(title, text, communityId);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.AreEqual("Community does not exists", badRequestResult.Value);
		}

		[Test]
		public async Task Remove_Returns_OkResult_When_Post_Exists()
		{
			// Arrange
			var postId = 1;
			_mockPostService.Setup(x => x.FindAsync(postId, "testUserId")).ReturnsAsync(new PostViewModel { Id = postId });
			_mockHubContext.Setup(x => x.Clients.Group(It.IsAny<string>())).Returns(Mock.Of<IClientProxy>());
			// Act
			var result = await _controller.Remove(postId);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<OkResult>(result);
		}

		[Test]
		public async Task Remove_Returns_BadRequest_When_Post_Not_Exists()
		{
			// Arrange
			var postId = 1;
			_mockPostService.Setup(x => x.FindAsync(postId, "testUserId")).ReturnsAsync((PostViewModel)null);

			// Act
			var result = await _controller.Remove(postId);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.AreEqual("Post does not exist!", badRequestResult.Value);
		}
		[Test]
		public async Task GetByDate_Returns_Posts_When_Community_Exists()
		{
			// Arrange
			var lastDate = DateTime.Now;
			var communityId = 1;
			var takeCount = 5;
			var accountId = "user123";
			var expectedPosts = new[] { new PostViewModel { Id = 1 }, new PostViewModel { Id = 2 } };
			_mockCommunityService.Setup(x => x.DoesExistAsync(communityId)).ReturnsAsync(true);
			_mockPostService.Setup(x => x.GetByDateAsync("testUserId", lastDate, communityId, takeCount)).ReturnsAsync(expectedPosts);

			// Act
			var result = await _controller.GetByDate(lastDate, communityId, takeCount);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<ActionResult<PostViewModel[]>>(result);
			var actionResult = (ActionResult<PostViewModel[]>)result;
			Assert.IsNotNull(actionResult.Value);
			Assert.AreEqual(expectedPosts, actionResult.Value);
		}

		[Test]
		public async Task GetByDate_Returns_BadRequest_When_Community_Does_Not_Exist()
		{
			// Arrange
			var lastDate = DateTime.Now;
			var communityId = 1;
			var takeCount = 5;
			_mockCommunityService.Setup(x => x.DoesExistAsync(communityId)).ReturnsAsync(false);

			// Act
			var result = await _controller.GetByDate(lastDate, communityId, takeCount);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
		}
		[Test]
		public async Task GetByCommunity_Returns_Posts_When_Community_Exists()
		{
			// Arrange
			var communityId = 1;
			var accountId = "user123";
			var expectedPosts = new[] { new PostViewModel { Id = 1 }, new PostViewModel { Id = 2 } };
			_mockCommunityService.Setup(x => x.DoesExistAsync(communityId)).ReturnsAsync(true);
			_mockPostService.Setup(x => x.GetByCommunityAsync(communityId)).ReturnsAsync(expectedPosts);

			// Act
			var result = await _controller.GetByCommunity(communityId);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<ActionResult<PostViewModel[]>>(result);
			var actionResult = (ActionResult<PostViewModel[]>)result;
			Assert.IsNotNull(actionResult.Value);
			Assert.AreEqual(expectedPosts, actionResult.Value);
		}

		[Test]
		public async Task GetByCommunity_Returns_BadRequest_When_Community_Does_Not_Exist()
		{
			// Arrange
			var communityId = 1;
			_mockCommunityService.Setup(x => x.DoesExistAsync(communityId)).ReturnsAsync(false);

			// Act
			var result = await _controller.GetByCommunity(communityId);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
		}
		[Test]
		public async Task GetBySearch_Returns_BadRequest_When_Search_Is_Empty()
		{
			// Arrange
			var search = "";
			var takeCount = 10;
			var communityId = 1;

			// Act
			var result = await _controller.GetBySearch(search, takeCount, communityId, null);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
		}

		[Test]
		public async Task GetBySearch_Returns_BadRequest_When_Community_Does_Not_Exist()
		{
			// Arrange
			var search = "test";
			var takeCount = 10;
			var communityId = 1;
			_mockCommunityService.Setup(x => x.DoesExistAsync(communityId)).ReturnsAsync(false);

			// Act
			var result = await _controller.GetBySearch(search, takeCount, communityId, null);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
		}

		[Test]
		public async Task GetBySearch_Returns_Posts_When_Search_Is_Valid_And_Community_Exists()
		{
			// Arrange
			var search = "test";
			var takeCount = 10;
			var communityId = 1;
			var expectedPosts = new[] { new PostViewModel { Id = 1 }, new PostViewModel { Id = 2 } };
			_mockCommunityService.Setup(x => x.DoesExistAsync(communityId)).ReturnsAsync(true);
			_mockPostService.Setup(x => x.GetBySearchAsync(search, It.IsAny<string>(), takeCount, communityId, null)).ReturnsAsync(expectedPosts);

			// Act
			var result = await _controller.GetBySearch(search, takeCount, communityId, null);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<ActionResult<PostViewModel[]>>(result);
			var actionResult = (ActionResult<PostViewModel[]>)result;
			Assert.IsNotNull(actionResult.Value);
			Assert.AreEqual(expectedPosts, actionResult.Value);
		}

		[Test]
		public async Task GetByTrending_Returns_Posts()
		{
			// Arrange
			var takeCount = 10;
			var communityId = 1;
			var skipIds = new int[] { 1, 2 };
			var expectedPosts = new[] { new PostViewModel { Id = 1 }, new PostViewModel { Id = 2 }, new PostViewModel { Id = 3 }, new PostViewModel { Id = 4 } };
			_mockPostService.Setup(x => x.GetTrendingAsync(skipIds, takeCount, It.IsAny<string>(), communityId)).ReturnsAsync(expectedPosts);
			_mockCommunityService.Setup(x => x.DoesExistAsync(communityId)).ReturnsAsync(true);
			// Act
			var result = await _controller.GetByTrending(takeCount, communityId, skipIds);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<ActionResult<PostViewModel[]>>(result);
			var actionResult = (ActionResult<PostViewModel[]>)result;
			Assert.IsNotNull(actionResult.Value);
			Assert.AreEqual(expectedPosts, actionResult.Value);
		}

		[Test]
		public async Task GetByFollowed_Returns_Posts()
		{
			// Arrange
			var takeCount = 10;
			var skipIds = new int[] { 1, 2 };
			var expectedPosts = new[] { new PostViewModel { Id = 1 }, new PostViewModel { Id = 2 } };
			_mockPostService.Setup(x => x.GetFollowedPostsAsync(skipIds, takeCount, It.IsAny<string>())).ReturnsAsync(expectedPosts);

			// Act
			var result = await _controller.GetByFollowed(takeCount, skipIds);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<ActionResult<PostViewModel[]>>(result);
			var actionResult = (ActionResult<PostViewModel[]>)result;
			Assert.IsNotNull(actionResult.Value);
			Assert.AreEqual(expectedPosts, actionResult.Value);
		}

	}
}
