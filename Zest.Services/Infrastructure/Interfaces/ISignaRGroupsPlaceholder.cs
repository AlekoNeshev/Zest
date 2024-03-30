using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zest.Services.Infrastructure.Interfaces
{
	public interface ISignaRGroupsPlaceholder
	{
		Task AddUserToGroup(string connectionId, string groupName);
	   void RemoveUserFromGroup(string connectionId, string groupName);
	}
}
