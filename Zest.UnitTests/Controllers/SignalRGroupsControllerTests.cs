using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.Controllers;
using Zest.Services.Infrastructure.Interfaces;

namespace Zest.UnitTests.UnitTests
{
	[TestFixture]
	public class SignalRGroupsControllerTests
	{
		private Mock<ISignaRService> _mockSignalRService;
		private SignalRGroupsController _controller;

		[SetUp]
		public void Setup()
		{
			_mockSignalRService = new Mock<ISignaRService>();
			_controller = new SignalRGroupsController(_mockSignalRService.Object);
		}

		[Test]
		public async Task AddConnectionToGroup_ShouldReturnOk()
		{
			// Arrange
			string connectionId = "testConnectionId";
			string[] groupsId = { "group1", "group2" };

			// Act
			var result = await _controller.AddConnectionToGroup(connectionId, groupsId);

			// Assert
			Assert.IsInstanceOf<OkResult>(result);
			_mockSignalRService.Verify(x => x.AddConnectionToGroup(connectionId, groupsId), Times.Once);
		}

		[Test]
		public async Task RemoveConnectionFromAllGroups_ShouldReturnOk()
		{
			// Arrange
			string connectionId = "testConnectionId";

			// Act
			var result = await _controller.RemoveConnectionFromAllGroups(connectionId);

			// Assert
			Assert.IsInstanceOf<OkResult>(result);
			_mockSignalRService.Verify(x => x.RemoveConnectionFromAllGroups(connectionId), Times.Once);
		}
	}
}
