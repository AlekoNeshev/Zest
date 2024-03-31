using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using System.Security.Claims;
using Xunit;
using Zest.Controllers;
using Zest.DBModels.Models;
using Zest.Services.Hubs;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;


[TestFixture]
public class CommentsTests
{
	private CommentController _controller;
	private Mock<ICommentsService> _commentsServiceMock;
	private Mock<IPostService> _postServiceMock;
	private Mock<IHubContext<DeleteHub>> _deleteHubContextMock;

	[SetUp]
	public void Setup()
	{
		_commentsServiceMock = new Mock<ICommentsService>();
		_postServiceMock = new Mock<IPostService>();
		_deleteHubContextMock = new Mock<IHubContext<DeleteHub>>();

		_controller = new CommentController(
			_commentsServiceMock.Object,
			_deleteHubContextMock.Object,
			_postServiceMock.Object);
	}

	[Test]
	public async Task Find_Returns_CommentViewModel_When_CommentExists()
	{
		// Arrange
		int commentId = 1;
		var accountId = "testAccountId";
		var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
		var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
		_controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

		var expectedComment = new CommentViewModel { Id = commentId, Text = "Test Comment" };
		_commentsServiceMock.Setup(x => x.FindAsync(commentId, accountId)).ReturnsAsync(expectedComment);

		// Act
		var result = await _controller.Find(commentId);

		// Assert
		NUnit.Framework.Assert.IsInstanceOf<ActionResult<CommentViewModel>>(result);
		var okResult = (ActionResult<CommentViewModel>)result;
		NUnit.Framework.Assert.AreEqual(expectedComment, okResult.Value);
	}
	[Test]
	public async Task Add_Returns_OkResult_When_CommentAddedSuccessfully()
	{
		// Arrange
		int postId = 1;
		int commentId = 0;
		var text = "Test comment";
		var accountId = "testAccountId";
		var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
		var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
		_controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
		_postServiceMock.Setup(x => x.DoesExist(postId)).ReturnsAsync(true);
		var newComment = new Comment { Id = 1, Text = text }; // Modify to match your Comment entity
		_commentsServiceMock.Setup(x => x.AddAsync(accountId, postId, text, commentId)).ReturnsAsync(newComment);

		// Act
		var result = await _controller.Add(postId, text, commentId);

		// Assert
		NUnit.Framework.Assert.IsInstanceOf<OkObjectResult>(result);
		var okResult = (OkObjectResult)result;
		NUnit.Framework.Assert.AreEqual(newComment.Id, okResult.Value);
	}

	[Test]
	public async Task Remove_Returns_OkResult_When_CommentRemovedSuccessfully()
	{
		// Arrange
		int commentId = 1;
		int postId = 1;
		_commentsServiceMock.Setup(x => x.DoesExist(commentId)).ReturnsAsync(true);
		_deleteHubContextMock.Setup(x => x.Clients.Group(It.IsAny<string>())).Returns(Mock.Of<IClientProxy>());
		// Act
		var result = await _controller.Remove(commentId, postId);

		// Assert
		NUnit.Framework.Assert.IsInstanceOf<OkObjectResult>(result);
		var okResult = (OkObjectResult)result;
		NUnit.Framework.Assert.AreEqual(commentId, okResult.Value);
	}

	[Test]
	public async Task GetCommentsByPost_Returns_CommentViewModelArray_When_PostExists()
	{
		// Arrange
		int postId = 1;
		DateTime lastDate = DateTime.Now;
		int takeCount = 10;
		var accountId = "testAccountId";
		var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
		var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
		_controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
		_postServiceMock.Setup(x => x.DoesExist(postId)).ReturnsAsync(true);
		var comments = new CommentViewModel[]
		{
		new CommentViewModel { Id = 1, Text = "Comment 1",},
		new CommentViewModel { Id = 2, Text = "Comment 2" }
		};
		_commentsServiceMock.Setup(x => x.GetCommentsByPostIdAsync(postId, lastDate, takeCount, accountId)).ReturnsAsync(comments);

		// Act
		var result = await _controller.GetCommentsByPost(postId, lastDate, takeCount);

		// Assert
		NUnit.Framework.Assert.IsInstanceOf<ActionResult<CommentViewModel[]>>(result);
		var okResult = (ActionResult<CommentViewModel[]>)result;
		NUnit.Framework.Assert.AreEqual(comments, okResult.Value);
	}

	[Test]
	public async Task GetByTrending_Returns_CommentViewModelArray_When_PostExists()
	{
		// Arrange
		int takeCount = 10;
		int postId = 1;
		int[] skipIds = new int[] { 2, 3 }; 
		var accountId = "testAccountId";
		var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
		var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
		_controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
		_postServiceMock.Setup(x => x.DoesExist(postId)).ReturnsAsync(true);
		var comments = new CommentViewModel[]
		{
		new CommentViewModel { Id = 1, Text = "Comment 1" },
		new CommentViewModel { Id = 2, Text = "Comment 2" }
		};
		_commentsServiceMock.Setup(x => x.GetTrendingCommentsAsync(skipIds, takeCount, accountId, postId)).ReturnsAsync(comments);

		// Act
		var result = await _controller.GetByTrending(takeCount, postId, skipIds);

		// Assert
		NUnit.Framework.Assert.IsInstanceOf<ActionResult<CommentViewModel[]>>(result);
		var okResult = (ActionResult<CommentViewModel[]>)result;
		NUnit.Framework.Assert.AreEqual(comments, okResult.Value);
	}
	[Test]
	public async Task GetByTrending_Returns_BadRequest_When_PostExists()
	{
		// Arrange
		int takeCount = 10;
		int postId = 1;
		_postServiceMock.Setup(x => x.DoesExist(postId)).ReturnsAsync(false);
		int[] skipIds = new int[] { 2, 3 }; // Example skipIds
		var accountId = "testAccountId";
		var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
		var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
		_controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
		
		var comments = new CommentViewModel[]
		{
		new CommentViewModel { Id = 1, Text = "Comment 1" },
		new CommentViewModel { Id = 2, Text = "Comment 2" }
		};
		_commentsServiceMock.Setup(x => x.GetTrendingCommentsAsync(skipIds, takeCount, accountId, postId)).ReturnsAsync(comments);
		
		// Act
		var result = await _controller.GetByTrending(takeCount, postId, skipIds);

		// Assert
		NUnit.Framework.Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
		
	}

}

