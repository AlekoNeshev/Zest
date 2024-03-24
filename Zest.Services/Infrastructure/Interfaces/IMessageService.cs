using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;
using Zest.ViewModels.ViewModels;

namespace Zest.Services.Infrastructure.Interfaces
{
	public interface IMessageService
	{
		Task<MessageViewModel> FindAsync(int id);
		Task<MessageViewModel[]> GetMessagesBySenderAndReceiverIdsAsync(string senderId, string receiverId, int takeCount, DateTime date);
		Task<MessageViewModel> AddAsync(string senderId, string receiverId, string text);
		Task RemoveAsync(int id);
		Task<MessageViewModel> FindBySenderIdAndReceiverIdAsync(string senderId, string receiverId);
	}
}
