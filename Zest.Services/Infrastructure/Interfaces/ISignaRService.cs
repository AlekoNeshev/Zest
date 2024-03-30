using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zest.Services.Infrastructure.Interfaces
{
	public interface ISignaRService
	{
		Task AddConnectionToGroup(string connectionId, string[]? groupsId);
		Task RemoveConnectionFromAllGroups(string connectionId);
	}
}
