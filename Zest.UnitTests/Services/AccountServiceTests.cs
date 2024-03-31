using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;
using Zest.DBModels;
using Zest.Services.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Zest.ViewModels.ViewModels;

namespace Zest.UnitTests.Services
{
	[TestFixture]
	public class AccountServiceTests
	{
		

		private AccountService _accountService;
		private ZestContext _zestContext;
		private IMapper _mapper;
		[SetUp]
		public void Setup()
		{
			var options = new DbContextOptionsBuilder<ZestContext>()
				.UseInMemoryDatabase(databaseName: "test_database")
				.Options;
			_zestContext = new ZestContext(options);
			_mapper = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<Account, AccountViewModel>();
				
			}).CreateMapper();
			_accountService = new AccountService(_zestContext, _mapper); 
		}

		[Test]
		public async Task DoesExistAsync_ReturnsTrue_WhenAccountExists()
		{
			// Arrange
			var accountId = "existingAccountId";
			await _zestContext.Accounts.AddAsync(new Account { Id = accountId, Username="testUsername", Email="testEmail" });
			await _zestContext.SaveChangesAsync();

			// Act
			var result = await _accountService.DoesExistAsync(accountId);

			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public async Task DoesExistAsync_ReturnsFalse_WhenAccountDoesNotExist()
		{
			// Arrange
			var accountId = "nonExistingAccountId";

			// Act
			var result = await _accountService.DoesExistAsync(accountId);

			// Assert
			Assert.IsFalse(result);
		}
		[Test]
		public async Task FindByIdAsync_ReturnsAccountViewModel_WhenIdExists()
		{
			// Arrange
			var accountId = "existingAccountId";
			await _zestContext.Accounts.AddAsync(new Account { Id = accountId, Username="testUsername", Email="testEmail" });
			await _zestContext.SaveChangesAsync();

			// Act
			var result = await _accountService.FindByIdAsync(accountId);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(accountId, result.Id);
		}

		[Test]
		public async Task FindByIdAsync_ReturnsNull_WhenIdDoesNotExist()
		{
			// Arrange
			var accountId = "nonExistingAccountId";

			// Act
			var result = await _accountService.FindByIdAsync(accountId);

			// Assert
			Assert.IsNull(result);
		}

		[Test]
		public async Task FindByUsernameAsync_ReturnsTrue_WhenUsernameExists()
		{
			// Arrange
			var username = "existingUsername";
			await _zestContext.Accounts.AddAsync(new Account { Id = "test",Username = username, Email="testEmail" });
			await _zestContext.SaveChangesAsync();

			// Act
			var result = await _accountService.FindByUsernameAsync(username);

			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public async Task FindByUsernameAsync_ReturnsFalse_WhenUsernameDoesNotExist()
		{
			// Arrange
			var username = "nonExistingUsername";

			// Act
			var result = await _accountService.FindByUsernameAsync(username);

			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public async Task AddAsync_AddsNewAccountAndReturnsAccountViewModel()
		{
			// Arrange
			var accountId = "testAccountId";
			var username = "testUsername";
			var email = "test@example.com";

			// Act
			var result = await _accountService.AddAsync(accountId, username, email);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(accountId, result.Id);
			Assert.AreEqual(username, result.Username);
			Assert.AreEqual(email, result.Email);
		}
	
	}
}
