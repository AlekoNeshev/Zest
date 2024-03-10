﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;
using Zest.DBModels;
using Zest.Services.Infrastructure.Interfaces;
using AutoMapper;
using Zest.ViewModels.ViewModels;

namespace Zest.Services.Infrastructure.Services
{
	public class CommunityService : ICommunityService
	{
		private readonly ZestContext _context;
		private readonly IMapper _mapper;

		public CommunityService(ZestContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<Community> GetCommunityByIdAsync(int id)
		{
			return await _context.Communities.FindAsync(id);
		}

		public async Task<Community[]> GetAllCommunitiesAsync(string accountId)
		{
			var communities = _context.Communities.ToArray();
			
			return communities;
		}

		public async Task<int> AddCommunityAsync(string creatorId, string name, string discription)
		{
			var community = new Community
			{
				Name = name,
				Information = discription,
				CreatorId = creatorId,
				CreatedOn = DateTime.Now,
			};
			await _context.AddAsync(community);
			await _context.SaveChangesAsync();
			return community.Id;
		}
		public async Task<CommunityViewModel[]> GetCommunitiesByAccount(string accountId)
		{
			return _mapper.Map<CommunityViewModel[]>(_context.CommunityFollowers.Where(x => x.AccountId==accountId).Select(x => x.Community).ToArray());
		}
	}
}
