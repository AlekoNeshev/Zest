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
using Zest.ViewModels.ViewModels;

namespace Zest.UnitTests.UnitTests
{
	[TestFixture]
	public class CommuntiesControllerTests
	{
		private Mock<ICommunityService> _communityServiceMock;

	
		private Mock<ICommunityFollowerService> _communityFollowerServiceMock;

	
		private Mock<IAccountService> _accountServiceMock;

		
		private CommunityController _controller;

		[SetUp]
		public void Setup()
		{
			
			_communityServiceMock = new Mock<ICommunityService>();
			_communityFollowerServiceMock = new Mock<ICommunityFollowerService>();
			_accountServiceMock = new Mock<IAccountService>();

			
			_controller = new CommunityController(_communityServiceMock.Object, _communityFollowerServiceMock.Object, _accountServiceMock.Object);
		}
		[Test]
		public async Task Find_Returns_CommunityViewModel_When_Exists()
		{
			// Arrange
			var communityId = 1;
			var accountId = "testAccountId";
			var community = new CommunityViewModel
			{
				Id = communityId,
				Name = "Community 1",
				Description = "Description",
				Creator = "Creator",
				CreatedOn = DateTime.Now,
				IsSubscribed = false
			};
			_communityServiceMock.Setup(x => x.GetCommunityByIdAsync(communityId, accountId)).ReturnsAsync(community);

		
			var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
			var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
			_controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

			// Act
			var result = await _controller.Find(communityId);

			// Assert
			Assert.IsInstanceOf<ActionResult<CommunityViewModel>>(result);
			var okResult = (ActionResult<CommunityViewModel>)result;
			Assert.AreEqual(community, okResult.Value);
		}
		[Test]
		public async Task GetAll_Returns_CommunityViewModelArray_When_Requested()
		{
			// Arrange
			var takeCount = 10;
			var accountId = "testAccountId";
			var communities = new CommunityViewModel[]
			{
		new CommunityViewModel { Id = 1, Name = "Community 1", Description = "Description 1", Creator = "Creator 1", CreatedOn = DateTime.Now, IsSubscribed = false },
		new CommunityViewModel { Id = 2, Name = "Community 2", Description = "Description 2", Creator = "Creator 2", CreatedOn = DateTime.Now, IsSubscribed = true }
			};
			_communityServiceMock.Setup(x => x.GetAllCommunitiesAsync(accountId, 0, takeCount)).ReturnsAsync(communities);

			
			var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
			var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
			_controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

			// Act
			var result = await _controller.GetAll(takeCount);

			// Assert
			Assert.IsInstanceOf<ActionResult<CommunityViewModel[]>>(result);
			var okResult = (ActionResult<CommunityViewModel[]>)result;
			Assert.AreEqual(communities, okResult.Value);
		}
		[Test]
		public async Task Add_Returns_OkResult_With_CommunityId()
		{
			// Arrange
			var accountId = "testAccountId";
			string communityName = "Test Community";
			string description = "Test Description";
			var expectedCommunityId = 1;
			var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
			var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
			_controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
			_communityServiceMock.Setup(x => x.AddCommunityAsync(accountId, communityName, description)).ReturnsAsync(expectedCommunityId);

			// Act
			var result = await _controller.Add(communityName, description);

			// Assert
			Assert.IsInstanceOf<ActionResult<int>>(result);
			var okResult = (ActionResult<int>)result;
			Assert.AreEqual(expectedCommunityId, okResult.Value);
		}

		[Test]
		public async Task Delete_Returns_OkResult_When_CommunityDeletedSuccessfully()
		{
			// Arrange
			int communityId = 1;
			_communityServiceMock.Setup(x => x.DoesExistAsync(communityId)).ReturnsAsync(true);
			var creatorId = "testCreatorId";
			var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, creatorId) };
			var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
			_controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

			// Act
			var result = await _controller.Delete(communityId);

			// Assert
			Assert.IsInstanceOf<OkResult>(result);
		}

		[Test]
		public async Task GetCommunitiesByAccount_Returns_CommunityViewModelArray()
		{
			// Arrange
			string accountId = "testAccountId";
			int takeCount = 10;
			int skipCount = 0;
			var communities = new[]
			{
		new CommunityViewModel { Id = 1, Name = "Community 1", Description = "Description 1", Creator = "Creator 1", IsSubscribed = true, CreatedOn = DateTime.Now },
		new CommunityViewModel { Id = 2, Name = "Community 2", Description = "Description 2", Creator = "Creator 2", IsSubscribed = false, CreatedOn = DateTime.Now }
	};
			_accountServiceMock.Setup(x => x.DoesExistAsync(accountId)).ReturnsAsync(true);
			_communityServiceMock.Setup(x => x.GetCommunitiesByAccount(accountId, takeCount, skipCount)).ReturnsAsync(communities);

			// Act
			var result = await _controller.GetCommunitiesByAccount(accountId, takeCount, skipCount);

			// Assert
			Assert.IsInstanceOf<ActionResult<CommunityViewModel[]>>(result);
			var okResult = (ActionResult<CommunityViewModel[]>)result;
			Assert.AreEqual(communities, okResult.Value);
		}

		[Test]
		public async Task GetCommunitiesByPopularity_Returns_CommunityViewModelArray()
		{
			// Arrange
			int takeCount = 10;
			int[] skipIds = new[] { 2, 3 }; 
			string accountId = "testAccountId";
			var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
			var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
			_controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
			var communities = new[]
			{
		new CommunityViewModel { Id = 1, Name = "Community 1", Description = "Description 1", Creator = "Creator 1", IsSubscribed = true, CreatedOn = DateTime.Now },
		new CommunityViewModel { Id = 2, Name = "Community 2", Description = "Description 2", Creator = "Creator 2", IsSubscribed = false, CreatedOn = DateTime.Now }
	};
			_communityServiceMock.Setup(x => x.GetTrendingCommunitiesAsync(skipIds, takeCount, accountId)).ReturnsAsync(communities);

			// Act
			var result = await _controller.GetCommunitiesByPopularity(takeCount, skipIds);

			// Assert
			Assert.IsInstanceOf<ActionResult<CommunityViewModel[]>>(result);
			var okResult = (ActionResult<CommunityViewModel[]>)result;
			Assert.AreEqual(communities, okResult.Value);
		}

		[Test]
		public async Task GetBySearch_Returns_CommunityViewModelArray()
		{
			// Arrange
			string search = "Test";
			int takeCount = 10;
			int[] skipIds = new[] { 2, 3 }; 
			string accountId = "testAccountId";
			var communities = new[]
			{
		new CommunityViewModel { Id = 1, Name = "Community 1", Description = "Description 1", Creator = "Creator 1", IsSubscribed = true, CreatedOn = DateTime.Now },
		new CommunityViewModel { Id = 2, Name = "Community 2", Description = "Description 2", Creator = "Creator 2", IsSubscribed = false, CreatedOn = DateTime.Now }
	};
			var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
			var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
			_controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
			_communityServiceMock.Setup(x => x.GetBySearchAsync(search, accountId, takeCount, skipIds)).ReturnsAsync(communities);

			// Act
			var result = await _controller.GetBySearch(search, takeCount, skipIds);

			// Assert
			Assert.IsInstanceOf<ActionResult<CommunityViewModel[]>>(result);
			var okResult = (ActionResult<CommunityViewModel[]>)result;
			Assert.AreEqual(communities, okResult.Value);
		}

	}
}
