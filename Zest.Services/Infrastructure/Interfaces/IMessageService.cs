using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;

namespace Zest.Services.Infrastructure.Interfaces
{
	public interface IMessageService
	{
		Task<Message> FindAsync(int id);
		Task<List<Message>> GetMessagesBySenderAndReceiverIdsAsync(string senderId, string receiverId, int takeCount, DateTime date);
		Task<Message> AddAsync(string senderId, string receiverId, string text);
		Task RemoveAsync(int id);
		Task<Message> FindBySenderIdAndReceiverIdAsync(string senderId, string receiverId);
	}
}
