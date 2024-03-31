using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Services.Infrastructure.Services;
using Zest.ViewModels.ViewModels;

namespace Zest.UnitTests.Services
{
	[TestFixture]
	public class MessageServiceTests
	{
		private MessageService _messageService;
		private ZestContext _context;

		[SetUp]
		public void Setup()
		{
			var options = new DbContextOptionsBuilder<ZestContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;
			_context = new ZestContext(options);
			_context.Database.EnsureCreated();

			var mapperConfig = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<Message, MessageViewModel>()
			   .ForMember(dest => dest.SenderUsername, op => op.MapFrom(src => src.Sender.Username))
			   .ForMember(dest => dest.Text, op => op.MapFrom(src => src.Text));
			});
			IMapper mapper = mapperConfig.CreateMapper();

			_messageService = new MessageService(_context, mapper);
		}

		[TearDown]
		public void TearDown()
		{
			_context.Dispose();
		}

		

		[Test]
		public async Task AddAsync_AddsMessage()
		{
			// Arrange
			var senderId = "senderId";
			var receiverId = "receiverId";
			var text = "Test message";

			// Act
			var result = await _messageService.AddAsync(senderId, receiverId, text);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(text, result.Text);
		}

		[Test]
		public async Task RemoveAsync_RemovesMessage()
		{
			// Arrange
			var message = new Message { Id = 1, SenderId = "senderId", ReceiverId = "receiverId", Text = "Test message", CreatedOn = DateTime.Now };
			await _context.Messages.AddAsync(message);
			await _context.SaveChangesAsync();

			// Act
			await _messageService.RemoveAsync(message.Id);
			var removedMessage = await _context.Messages.FindAsync(message.Id);

			// Assert
			Assert.IsNull(removedMessage);
		}

		
	}
}
