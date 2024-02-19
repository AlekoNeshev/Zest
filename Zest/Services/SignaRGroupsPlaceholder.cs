using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using Zest.Hubs;

namespace Zest.Services
{
	public class SignaRGroupsPlaceholder
	{
		private readonly IDictionary<string, HashSet<string>> _userGroups = new Dictionary<string, HashSet<string>>();
		

		public SignaRGroupsPlaceholder()
		{
			
		}

		public async Task AddUserToGroup(string connectionId, string groupName)
		{
			lock (_userGroups)
			{
				if (!_userGroups.ContainsKey(connectionId))
				{
					_userGroups[connectionId] = new HashSet<string>();
				}
				_userGroups[connectionId].Add(groupName);
			   
			}
			
		}

		public void RemoveUserFromGroup(string connectionId, string groupName)
		{
			lock (_userGroups)
			{
				if (_userGroups.ContainsKey(connectionId))
				{
					_userGroups[connectionId].Remove(groupName);
					if (_userGroups[connectionId].Count == 0)
					{
						_userGroups.Remove(connectionId);
					}
				}
			}
		}
		public void RemoveUserFromAllGroups(string connectionId)
		{
			lock (_userGroups)
			{
				if (_userGroups.ContainsKey(connectionId))
				{
					_userGroups[connectionId].Clear();
					
				}
			}
		}
		public async Task<string[]> RetrieveGroups(string connectionId)
		{
			lock (_userGroups)
			{
				if (_userGroups.ContainsKey(connectionId))
				{
					return _userGroups[connectionId].ToArray();

				}
			}
			return new string[0];
		}

	}
}
