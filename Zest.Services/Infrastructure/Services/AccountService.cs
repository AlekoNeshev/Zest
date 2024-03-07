using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;
using Zest.DBModels;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Zest.Services.Infrastructure.Services
{
	public class AccountService : IAccountService
	{
		private readonly ZestContext _zestContext;
		private readonly IMapper _mapper;

		public AccountService(ZestContext zestContext, IMapper mapper)
		{
			_zestContext = zestContext;
			_mapper = mapper;
		}

		public async Task<Account> FindByIdAsync(string id)
		{
			var account =_zestContext.Accounts.Where(x => x.Id == id).FirstOrDefault();
			return account;
		}

		public async Task<EntityEntry<Account>> AddAsync(string accountId,string username, string email)
		{
			var newAccount = await _zestContext.AddAsync(new Account
			{
				Id = accountId,
				Username = username,
				Email = email,
				CreatedOn = DateTime.Now,
				IsAdmin = false
			});
			 await _zestContext.SaveChangesAsync();
			
			return newAccount;
		}

		public async Task<Account[]> GetAllAsync(string accountId)
		{
			
			var accounts = _zestContext.Accounts.ToArray();
			
			return accounts;
		}
	}

}
