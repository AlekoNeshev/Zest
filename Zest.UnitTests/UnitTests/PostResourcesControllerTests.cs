using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.Controllers;
using Zest.Services.ActionResult;
using Zest.Services.Infrastructure.Interfaces;

namespace Zest.UnitTests.UnitTests
{
	[TestFixture]
	public class PostResourcesControllerTests
	{
		private Mock<IPostResourcesService> _mockPostResourcesService;
		private Mock<IPostService> _mockPostService;
		private PostRescourcesController _controller;

		[SetUp]
		public void Setup()
		{
			_mockPostResourcesService = new Mock<IPostResourcesService>();
			_mockPostService = new Mock<IPostService>();
			_controller = new PostRescourcesController(_mockPostResourcesService.Object, _mockPostService.Object);
		}

		[Test]
		public async Task UploadFile_Returns_BadRequest_When_Post_Does_Not_Exist()
		{
			// Arrange
			int postId = 1;
			_mockPostService.Setup(x => x.DoesExist(postId)).ReturnsAsync(false);

			// Act
			var result = await _controller.UploadFile(postId, null);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.AreEqual("Post does not exist!", badRequestResult.Value);
		}

		
		[Test]
		public async Task GetPhotos_Returns_BadRequest_When_Post_Does_Not_Exist()
		{
			// Arrange
			int postId = 1;
			_mockPostService.Setup(x => x.DoesExist(postId)).ReturnsAsync(false);

			// Act
			var result = await _controller.GetPhotos(postId);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
		}

		
	}
}
