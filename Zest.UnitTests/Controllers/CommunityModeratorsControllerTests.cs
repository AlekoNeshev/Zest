using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
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
	public class CommunityModeratorsControllerTests
	{
		private Mock<ICommunityModeratorService> _mockCommunityModeratorService;
		private Mock<ICommunityService> _mockCommunityService;
		private Mock<IAccountService> _mockAccountService;
		private CommunityModeratorsController _controller;

		[SetUp]
		public void Setup()
		{
			_mockCommunityModeratorService = new Mock<ICommunityModeratorService>();
			_mockCommunityService = new Mock<ICommunityService>();
			_mockAccountService = new Mock<IAccountService>();
			_controller = new CommunityModeratorsController(_mockCommunityModeratorService.Object, _mockCommunityService.Object, _mockAccountService.Object);
		}

		
		[Test]
		public async Task Add_ShouldReturnBadRequest_WhenUserIsCandidate()
		{
			// Arrange
			var accountId = "testAccountId";
			var communityId = 1;
			var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
			var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
			_controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
			_mockCommunityModeratorService.Setup(x => x.IsModeratorCandidateAsync(accountId, communityId)).ReturnsAsync(true);

			// Act
			var result = await _controller.Add(accountId, communityId);

			// Assert
			Assert.IsInstanceOf<BadRequestObjectResult>(result);
			var badRequestResult = result as BadRequestObjectResult;
			Assert.AreEqual("User is already candidate", badRequestResult.Value);
		}

		[Test]
		public async Task Add_ShouldCallAddModeratorAsync_WhenUserIsNotCandidate()
		{
			// Arrange
			var accountId = "testAccountId";
			var communityId = 1;
			var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
			var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
			_controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
			_mockCommunityModeratorService.Setup(x => x.IsModeratorCandidateAsync(accountId, communityId)).ReturnsAsync(false);

			// Act
			var result = await _controller.Add(accountId, communityId);

			// Assert
			_mockCommunityModeratorService.Verify(x => x.AddModeratorAsync(accountId, communityId), Times.Once);
			Assert.IsInstanceOf<OkResult>(result);
		}
		[Test]
		public async Task GetModeratorsByCommunity_ShouldReturnBadRequest_WhenCommunityDoesNotExist()
		{
			// Arrange
			var communityId = 1;
			
			_mockCommunityService.Setup(x => x.DoesExistAsync(communityId)).ReturnsAsync(false);

			// Act
			var result = await _controller.GetModeratorsByCommunity(communityId);

			// Assert
			Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
			var badRequestResult = result.Result as BadRequestObjectResult;
			Assert.AreEqual("Community does not exists", badRequestResult.Value);
		}

		[Test]
		public async Task GetModeratorCandidatesByCommunity_ShouldReturnBadRequest_WhenCommunityDoesNotExist()
		{
			// Arrange
			var communityId = 1;
			_mockCommunityService.Setup(x => x.DoesExistAsync(communityId)).ReturnsAsync(false);

			// Act
			var result = await _controller.GetModeratorCandidatesByCommunity(communityId);

			// Assert
			Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
			var badRequestResult = result.Result as BadRequestObjectResult;
			Assert.AreEqual("Community does not exists", badRequestResult.Value);
		}

		[Test]
		public async Task ApproveCandidate_ShouldReturnBadRequest_WhenAccountDoesNotExist()
		{
			// Arrange
			var accountId = "testAccountId";
			var communityId = 1;
			_mockAccountService.Setup(x => x.DoesExistAsync(accountId)).ReturnsAsync(false);

			// Act
			var result = await _controller.ApproveCandidate(accountId, communityId);

			// Assert
			Assert.IsInstanceOf<BadRequestObjectResult>(result);
			var badRequestResult = result as BadRequestObjectResult;
			Assert.AreEqual("Account does not exists", badRequestResult.Value);
		}

		[Test]
		public async Task ApproveCandidate_ShouldReturnBadRequest_WhenCommunityDoesNotExist()
		{
			// Arrange
			var accountId = "testAccountId";
			var communityId = 1;
			_mockAccountService.Setup(x => x.DoesExistAsync(accountId)).ReturnsAsync(true);
			_mockCommunityService.Setup(x => x.DoesExistAsync(communityId)).ReturnsAsync(false);

			// Act
			var result = await _controller.ApproveCandidate(accountId, communityId);

			// Assert
			Assert.IsInstanceOf<BadRequestObjectResult>(result);
			var badRequestResult = result as BadRequestObjectResult;
			Assert.AreEqual("Community does not exists", badRequestResult.Value);
		}

		[Test]
		public async Task RemoveModerator_ShouldReturnBadRequest_WhenAccountDoesNotExist()
		{
			// Arrange
			var accountId = "testAccountId";
			var communityId = 1;
			_mockAccountService.Setup(x => x.DoesExistAsync(accountId)).ReturnsAsync(false);

			// Act
			var result = await _controller.RemoveModerator(accountId, communityId);

			// Assert
			Assert.IsInstanceOf<BadRequestObjectResult>(result);
			var badRequestResult = result as BadRequestObjectResult;
			Assert.AreEqual("Account does not exists", badRequestResult.Value);
		}

		[Test]
		public async Task RemoveModerator_ShouldReturnBadRequest_WhenCommunityDoesNotExist()
		{
			// Arrange
			var accountId = "testAccountId";
			var communityId = 1;
			_mockAccountService.Setup(x => x.DoesExistAsync(accountId)).ReturnsAsync(true);
			_mockCommunityService.Setup(x => x.DoesExistAsync(communityId)).ReturnsAsync(false);

			// Act
			var result = await _controller.RemoveModerator(accountId, communityId);

			// Assert
			Assert.IsInstanceOf<BadRequestObjectResult>(result);
			var badRequestResult = result as BadRequestObjectResult;
			Assert.AreEqual("Community does not exists", badRequestResult.Value);
		}
		
	}
}
