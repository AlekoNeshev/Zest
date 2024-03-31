using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Zest.Controllers;
using Zest.DBModels.Models;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

[TestFixture]
public class AccountControllerTests
{
	private AccountController _controller;
	private Mock<IAccountService> _accountServiceMock;

	[SetUp]
	public void Setup()
	{
		_accountServiceMock = new Mock<IAccountService>();
		_controller = new AccountController(_accountServiceMock.Object);
	}
	[Test]
	public async Task FindById_Returns_AccountViewModel_When_IdExists()
	{
		// Arrange
		string accountId = "testAccountId";
		var expectedAccount = new AccountViewModel { Id = accountId, Username = "testUser", Email = "test@example.com", IsAdmin = false };
		var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
		var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
		_controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
		_accountServiceMock.Setup(x => x.FindByIdAsync(accountId)).ReturnsAsync(expectedAccount);

		// Act
		var result = await _controller.FindById();

		// Assert
		NUnit.Framework.Assert.IsInstanceOf<ActionResult<AccountViewModel>>(result);
		var okResult = (ActionResult<AccountViewModel>)result;
		NUnit.Framework.Assert.AreEqual(expectedAccount, okResult.Value);
	}
	[Test]
	public async Task Add_Returns_OkResult_When_AccountAddedSuccessfully()
	{
		// Arrange
		string accountId = "testAccountId";
		string username = "testUser";
		string email = "test@example.com";
		var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
		var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
		_controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
		var expectedAccount = new AccountViewModel { Id = accountId, Username = username, Email = email, IsAdmin = false };
		_accountServiceMock.Setup(x => x.AddAsync(accountId, username, email)).ReturnsAsync(expectedAccount);
		
		
		// Act
		var result = await _controller.Add(username, email);

		// Assert
		NUnit.Framework.Assert.IsInstanceOf<ActionResult<AccountViewModel>>(result);
		var okResult = (ActionResult<AccountViewModel>)result;
		NUnit.Framework.Assert.AreEqual(expectedAccount, okResult.Value);
	}
	[Test]
	public async Task GetAll_Returns_UserViewModelArray_When_Requested()
	{
		// Arrange
		var takeCount = 10;
		var accountId = "testAccountId";
		var accounts = new UserViewModel[]
		{
		new UserViewModel { Id = "1", Username = "User1" },
		new UserViewModel { Id = "2", Username = "User2" }
		};

		
		var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
		var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
		_controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

		_accountServiceMock.Setup(x => x.GetAllAsync(accountId, takeCount, It.IsAny<int>())).ReturnsAsync(accounts);

		// Act
		var result = await _controller.GetAll(takeCount);

		// Assert
		NUnit.Framework.Assert.IsInstanceOf<ActionResult<UserViewModel[]>>(result);
		var okResult = (ActionResult<UserViewModel[]>)result;
		NUnit.Framework.Assert.AreEqual(accounts, okResult.Value);
	}

	[Test]
	public async Task GetBySearch_Returns_UserViewModelArray_When_Requested()
	{
		// Arrange
		var search = "Test";
		var takeCount = 10;
		var accountId = "testAccountId";
		var skipIds = new string[] { "2", "3" };
		var accounts = new UserViewModel[]
		{
		new UserViewModel { Id = "1", Username = "User1" },
		new UserViewModel { Id = "2", Username = "User2" }
		};

		
		var userClaims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, accountId) };
		var user = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
		_controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

		_accountServiceMock.Setup(x => x.GetBySearchAsync(search, accountId, takeCount, skipIds)).ReturnsAsync(accounts);

		// Act
		var result = await _controller.GetBySearch(search, takeCount, skipIds);

		// Assert
		NUnit.Framework.Assert.IsInstanceOf<ActionResult<UserViewModel[]>>(result);
		var okResult = (ActionResult<UserViewModel[]>)result;
		NUnit.Framework.Assert.AreEqual(accounts, okResult.Value);
	}

}

