using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

namespace Zest.Services.Infrastructure.Services
{
	public class MessageService : IMessageService
	{
		private readonly ZestContext _context;
		private readonly IMapper _mapper;
		public MessageService(ZestContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<MessageViewModel> FindAsync(int id)
		{
			return _mapper.Map<MessageViewModel>(await _context.Messages.Include(x => x.Sender).Include(x => x.Receiver).FirstOrDefaultAsync(x=>x.Id == id));
		}

		public async Task<MessageViewModel[]> GetMessagesBySenderAndReceiverIdsAsync(string senderId, string receiverId, int takeCount, DateTime date)
		{
			var messagesFromSender = await _context.Messages.Where(x => x.SenderId == senderId && x.ReceiverId == receiverId && x.CreatedOn < date).Include(x => x.Sender).Include(x => x.Receiver).ToArrayAsync();
			var messagesFromReceiver = await _context.Messages.Where(x => x.SenderId == receiverId && x.ReceiverId == senderId && x.CreatedOn < date).Include(x => x.Sender).Include(x => x.Receiver).ToArrayAsync();

			var messages = new List<Message>(messagesFromSender.Length + messagesFromReceiver.Length);
			messages.AddRange(messagesFromSender);
			messages.AddRange(messagesFromReceiver);
			messages = messages.OrderByDescending(x=>x.CreatedOn).ToList();
			return _mapper.Map<MessageViewModel[]>(messages.Take(takeCount).ToArray());
		}

		public async Task<MessageViewModel> AddAsync(string senderId, string receiverId, string text)
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

			return _mapper.Map<MessageViewModel>(message);
		}

		public async Task RemoveAsync(int id)
		{
			var message = await _context.Messages.FindAsync(id);
			_context.Messages.Remove(message);
			await _context.SaveChangesAsync();
		}

		public async Task<MessageViewModel> FindBySenderIdAndReceiverIdAsync(string senderId, string receiverId)
		{
			return _mapper.Map<MessageViewModel>( await _context.Messages.Include(x => x.Sender).Include(x => x.Receiver).FirstOrDefaultAsync(m => m.SenderId == senderId && m.ReceiverId == receiverId));
		}
	}
}
