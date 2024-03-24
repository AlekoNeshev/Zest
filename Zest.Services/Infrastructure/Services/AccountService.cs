using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

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

		public async Task<AccountViewModel> FindByIdAsync(string id)
		{
			var account =_mapper.Map<AccountViewModel>(await _zestContext.Accounts.Where(x => x.Id == id).FirstOrDefaultAsync());
			return account;
		}

		public async Task<AccountViewModel> AddAsync(string accountId,string username, string email)
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
			
			return _mapper.Map<AccountViewModel>(newAccount.Entity); ;
		}

		public async Task<UserViewModel[]> GetAllAsync(string accountId)
		{
			
			var accounts = _mapper.Map<UserViewModel[]>(await _zestContext.Accounts.Where(x=>x.Id != accountId).ToArrayAsync());
			foreach (var item in accounts)
			{


				item.IsFollowed = await FindFollowerAsync(accountId, item.Id) != null;
			}
			return accounts;
		}
		private async Task<Follower> FindFollowerAsync(string followerId, string followedId)
		{
			return await _zestContext.Followers.Include(x => x.Followed).Include(x => x.FollowerNavigation).FirstOrDefaultAsync(x => x.FollowerId == followerId && x.FollowedId == followedId);
		}
	}

}
