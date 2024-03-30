using Zest.Services.Infrastructure.Interfaces;

namespace Zest.Services.Infrastructure.Services
{
	public class SignaRGroupsPlaceholder : ISignaRGroupsPlaceholder
	{
		private readonly IDictionary<string, HashSet<string>> _userGroups = new Dictionary<string, HashSet<string>>();

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
