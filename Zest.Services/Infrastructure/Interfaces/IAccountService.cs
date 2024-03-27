using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;
using Zest.ViewModels.ViewModels;

namespace Zest.Services.Infrastructure.Interfaces
{
	public interface IAccountService
	{
		Task<AccountViewModel> FindByIdAsync(string id);
		Task<AccountViewModel> AddAsync(string accountId, string name, string email);
		Task<UserViewModel[]> GetAllAsync(string accountId);
		Task<UserViewModel[]> GetBySearchAsync(string search, string accountId);
	}
}
