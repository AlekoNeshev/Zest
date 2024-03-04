using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Hubs;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private ZestContext context;
        IMapper mapper;
		private IHubContext<MessageHub> hubContext;
		public MessagesController(ZestContext context, IMapper mapper, IHubContext<MessageHub> hubContext)
        {
            this.context=context;
            this.mapper=mapper;
            this.hubContext=hubContext;
        }
        [Route("get/{id}")]
        [HttpGet]
        public async Task<ActionResult<MessageViewModel>> Find(int id)
        {
            return mapper.Map<MessageViewModel>(context.Messages.Find(id));
        }
        [Route("get/receiver/{receiverId}")]
        [HttpGet]
		public async Task<ActionResult<MessageViewModel[]>> Add(string receiverId)
		{
			var user = User.Claims;
			var senderId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			Message[] messagesFromSender = context.Messages.Where(x => x.SenderId==senderId && x.ReceiverId==receiverId).ToArray();
			Message[] messagesFromReceiver = context.Messages.Where(x => x.SenderId==receiverId && x.ReceiverId==senderId).ToArray();
            Message[] messages = new Message[messagesFromSender.Length + messagesFromReceiver.Length];
            Array.Copy(messagesFromSender, messages, messagesFromSender.Length);
            Array.Copy(messagesFromReceiver, 0, messages, messagesFromSender.Length, messagesFromReceiver.Length);
			return mapper.Map<MessageViewModel[]>(messages.OrderBy(x=>x.CreatedOn).ToArray());
		}
		[Route("add/receiver/{receiverId}")]
        [HttpPost]
        public async Task<ActionResult> Add( string receiverId,[FromBody] string text)
        {
			var user = User.Claims;
			var senderId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var message = context.Add(new Message { SenderId = senderId, ReceiverId = receiverId, Text = text, CreatedOn = DateTime.Now });

            context.SaveChanges();
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
			var messageId = message.Property<int>("Id").CurrentValue;
			await hubContext.Clients.Groups($"chat-{firstHubId}{secondHubId}").SendAsync("MessageSent", messageId);
			
			return Ok(messageId);
        }
        [Route("remove/receiver/{receiverId}")]
        [HttpDelete]
        public async Task<ActionResult> Remove(string receiverId)
        {
			var user = User.Claims;
			var senderId = user.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			Message message = context.Messages.FirstOrDefault(m=>m.SenderId==senderId && m.ReceiverId==receiverId);
            context.Messages.Remove(message);
            context.SaveChanges();
            return Ok();
        }
    }

}
