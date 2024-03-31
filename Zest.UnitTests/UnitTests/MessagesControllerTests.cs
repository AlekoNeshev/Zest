using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System.Security.Claims;
using Zest.Controllers;
using Zest.Services.Hubs;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

namespace Zest.UnitTests.UnitTests
{
	[TestFixture]
	public class MessagesControllerTests
	{
		private Mock<IMessageService> _mockMessageService;
		private Mock<IHubContext<MessageHub>> _mockHubContext;
		private Mock<IAccountService> _mockAccountService;
		private MessagesController _controller;

		[SetUp]
		public void Setup()
		{
			_mockMessageService = new Mock<IMessageService>();
			_mockHubContext = new Mock<IHubContext<MessageHub>>();
			_mockAccountService = new Mock<IAccountService>();
			_controller = new MessagesController(_mockMessageService.Object, _mockHubContext.Object, _mockAccountService.Object);
		}

		[Test]
		public async Task Find_ShouldReturnMessage_WhenValidIdProvided()
		{
			// Arrange
			var messageId = 1;
			var expectedMessage = new MessageViewModel { Id = messageId, SenderUsername = "Sender", Text = "Test message", CreatedOn = DateTime.Now };
			_mockMessageService.Setup(x => x.FindAsync(messageId)).ReturnsAsync(expectedMessage);

			// Act
			var result = await _controller.Find(messageId);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<ActionResult<MessageViewModel>>(result);
			Assert.IsNotNull(result.Value);
			Assert.AreEqual(expectedMessage, result.Value);
		}

		[Test]
		public async Task GetMessagesByReceiverId_ShouldReturnMessages_WhenValidReceiverIdProvided()
		{
			// Arrange
			string accountId = "testAccount";
			var receiverId = "receiverId";
			var takeCount = 10;
			var date = DateTime.Now;
			var expectedMessages = new MessageViewModel[]
			{
				new MessageViewModel { Id = 1, SenderUsername = "Sender1", Text = "Test message 1", CreatedOn = DateTime.Now },
				new MessageViewModel { Id = 2, SenderUsername = "Sender2", Text = "Test message 2", CreatedOn = DateTime.Now }
			};
			var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
			var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = user }
			};
			_mockAccountService.Setup(x => x.DoesExistAsync(receiverId)).ReturnsAsync(true);
			_mockMessageService.Setup(x => x.GetMessagesBySenderAndReceiverIdsAsync(accountId, receiverId, takeCount, date)).ReturnsAsync(expectedMessages);
			_mockHubContext.Setup(x => x.Clients.Group(It.IsAny<string>())).Returns(Mock.Of<IClientProxy>());
			// Act
			var result = await _controller.GetMessagesByReceiverId(receiverId, takeCount, date);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<ActionResult<MessageViewModel[]>>(result);
			Assert.IsNotNull(result.Value);
			Assert.AreEqual(expectedMessages, result.Value);
		}

		[Test]
		public async Task Add_ShouldReturnOk_WhenValidReceiverIdAndTextProvided()
		{
			// Arrange
			var receiverId = "receiverId";
			var text = "Test message";
			var expectedMessageId = 1;
			var accountId = "accountId";
			var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
			var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = user }
			};
			
			_mockAccountService.Setup(x => x.DoesExistAsync(receiverId)).ReturnsAsync(true);
			_mockMessageService.Setup(x => x.AddAsync(accountId, receiverId, text)).ReturnsAsync(new MessageViewModel { Id = expectedMessageId });
			_mockHubContext.Setup(x => x.Clients.Group(It.IsAny<string>())).Returns(Mock.Of<IClientProxy>());
			// Act
			var result = await _controller.Add(receiverId, text);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsInstanceOf<OkObjectResult>(result);
			var okResult = (OkObjectResult)result;
			Assert.AreEqual(expectedMessageId, okResult.Value);
		}
	}
}
