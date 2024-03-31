using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Zest.Services.Hubs;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
	[Authorize]
	[Route("Zest/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
		private readonly IMessageService _messageService;
		private readonly IHubContext<MessageHub> _hubContext;
		private readonly IAccountService _accountService;
		public MessageController(IMessageService messageService, IHubContext<MessageHub> hubContext, IAccountService accountService)
		{
			_messageService = messageService;
			_hubContext = hubContext;
			_accountService=accountService;
		}

		[Route("get/{id}")]
		[HttpGet]
		public async Task<ActionResult<MessageViewModel>> Find(int id)
		{
			var message = await _messageService.FindAsync(id);
			return message;
		}

		[Route("get/receiver/{receiverId}/{takeCount}/{date}")]
		[HttpGet]
		public async Task<ActionResult<MessageViewModel[]>> GetMessagesByReceiverId(string receiverId, int takeCount, [FromRoute]DateTime date)
		{
			var senderId = User.Id();
			var doesAccountExists = await _accountService.DoesExistAsync(receiverId);
			if (!doesAccountExists)
			{
				return BadRequest("Account does not exists");
			}
			var messages = await _messageService.GetMessagesBySenderAndReceiverIdsAsync(senderId, receiverId, takeCount, date);
			return messages.ToArray();
		}

		[Route("add/receiver/{receiverId}")]
		[HttpPost]
		public async Task<ActionResult> Add(string receiverId, [FromBody] string text)
		{
			var senderId = User.Id();
			var doesAccountExists = await _accountService.DoesExistAsync(receiverId);
			if (!doesAccountExists)
			{
				return BadRequest("Account does not exists");
			}

			var message = await _messageService.AddAsync(senderId, receiverId, text);

			int comparisonResult = string.Compare(senderId, receiverId);
			string firstHubId, secondHubId;

			if (comparisonResult >= 0)
			{
				firstHubId = senderId;
				secondHubId = receiverId;
			}
			else
			{
				firstHubId = receiverId;
				secondHubId = senderId;
			}

			await _hubContext.Clients.Group($"chat-{firstHubId}{secondHubId}").SendAsync("MessageSent", message.Id);

			return Ok(message.Id);
		}

		
	}

}
