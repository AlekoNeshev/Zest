using AutoMapper;
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
	[Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
		private readonly IMessageService _messageService;
		private readonly IHubContext<MessageHub> _hubContext;
		private readonly IMapper _mapper;

		public MessagesController(IMessageService messageService, IHubContext<MessageHub> hubContext, IMapper mapper)
		{
			_messageService = messageService;
			_hubContext = hubContext;
			_mapper = mapper;
		}

		[Route("get/{id}")]
		[HttpGet]
		public async Task<ActionResult<MessageViewModel>> Find(int id)
		{
			var message = await _messageService.FindAsync(id);
			return _mapper.Map<MessageViewModel>(message);
		}

		[Route("get/receiver/{receiverId}/{takeCount}/{date}")]
		[HttpGet]
		public async Task<ActionResult<MessageViewModel[]>> GetMessagesByReceiverId(string receiverId, int takeCount, [FromRoute]DateTime date)
		{
			var user = User.Claims;
			var senderId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var messages = await _messageService.GetMessagesBySenderAndReceiverIdsAsync(senderId, receiverId, takeCount, date);
			return _mapper.Map<MessageViewModel[]>(messages.ToArray());
		}

		[Route("add/receiver/{receiverId}")]
		[HttpPost]
		public async Task<ActionResult> Add(string receiverId, [FromBody] string text)
		{
			var user = User.Claims;
			var senderId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
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

			await _hubContext.Clients.Groups($"chat-{firstHubId}{secondHubId}").SendAsync("MessageSent", message.Id);

			return Ok(message.Id);
		}

		[Route("remove/receiver/{receiverId}")]
		[HttpDelete]
		public async Task<ActionResult> Remove(string receiverId)
		{
			var user = User.Claims;
			var senderId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var message = await _messageService.FindBySenderIdAndReceiverIdAsync(senderId, receiverId);
			await _messageService.RemoveAsync(message.Id);
			return Ok();
		}
	}

}
