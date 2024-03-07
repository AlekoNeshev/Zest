﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;
using Zest.DBModels;
using Zest.Services.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Zest.Services.Infrastructure.Services
{
	public class MessageService : IMessageService
	{
		private readonly ZestContext _context;

		public MessageService(ZestContext context)
		{
			_context = context;
		}

		public async Task<Message> FindAsync(int id)
		{
			return await _context.Messages.FindAsync(id);
		}

		public async Task<List<Message>> GetMessagesBySenderAndReceiverIdsAsync(string senderId, string receiverId)
		{
			var messagesFromSender = await _context.Messages.Where(x => x.SenderId == senderId && x.ReceiverId == receiverId).ToListAsync();
			var messagesFromReceiver = await _context.Messages.Where(x => x.SenderId == receiverId && x.ReceiverId == senderId).ToListAsync();

			var messages = new List<Message>(messagesFromSender.Count + messagesFromReceiver.Count);
			messages.AddRange(messagesFromSender);
			messages.AddRange(messagesFromReceiver);

			return messages;
		}

		public async Task<Message> AddAsync(string senderId, string receiverId, string text)
		{
			var message = new Message
			{
				SenderId = senderId,
				ReceiverId = receiverId,
				Text = text,
				CreatedOn = DateTime.Now
			};

			await _context.AddAsync(message);
			await _context.SaveChangesAsync();

			return message;
		}

		public async Task RemoveAsync(int id)
		{
			var message = await _context.Messages.FindAsync(id);
			_context.Messages.Remove(message);
			await _context.SaveChangesAsync();
		}

		public async Task<Message> FindBySenderIdAndReceiverIdAsync(string senderId, string receiverId)
		{
			return await _context.Messages.FirstOrDefaultAsync(m => m.SenderId == senderId && m.ReceiverId == receiverId);
		}
	}
}
