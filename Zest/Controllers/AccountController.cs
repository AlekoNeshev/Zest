﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zest.Services.Infrastructure.Interfaces;
using Zest.Services.Infrastructure.Services;
using Zest.ViewModels.ViewModels;
namespace Zest.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountService _accountService;
        
        public AccountController(IAccountService accountService)
        {
            this._accountService = accountService;      
        }
 
        [Route("get")]
        [HttpGet]
        public async Task<ActionResult<AccountViewModel>> FindById()
        {
            var accountId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var account = await _accountService.FindByIdAsync(accountId);

            return Ok(account);
        }
       
        [Route("add/{name}/{email}")]
        [HttpPost]
        public async Task<ActionResult<AccountViewModel>> Add(string name, string email)
        {
            var accountId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var account = _accountService.AddAsync(accountId, name, email);
   
            return Ok(account);
        }
		[Route("getAll")]
		[HttpGet]
		public async Task<ActionResult<UserViewModel[]>> GetAll()
		{
			
			var accountId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var accounts = await _accountService.GetAllAsync(accountId);
           			
			return Ok(accounts);
		}
		[Route("getBySearch/{search}/{takeCount}")]
		[HttpPost]
		public async Task<ActionResult<UserViewModel[]>> GetBySearch(string search, int takeCount, [FromBody] string[]? skipIds)
		{
			var user = User.Claims;
			var accountId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var accounts = await _accountService.GetBySearchAsync(search, accountId, takeCount, skipIds);

			return accounts;
		}
	}
}
