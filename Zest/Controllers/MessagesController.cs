using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Hubs;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
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
        [Route("get/{senderId}/receiver/{receiverId}")]
        [HttpGet]
		public async Task<ActionResult<MessageViewModel[]>> Add(int senderId, int receiverId)
		{
            Message[] messagesFromSender = context.Messages.Where(x => x.SenderId==senderId && x.ReceiverId==receiverId).ToArray();
			Message[] messagesFromReceiver = context.Messages.Where(x => x.SenderId==receiverId && x.ReceiverId==senderId).ToArray();
            Message[] messages = new Message[messagesFromSender.Length + messagesFromReceiver.Length];
            Array.Copy(messagesFromSender, messages, messagesFromSender.Length);
            Array.Copy(messagesFromReceiver, 0, messages, messagesFromSender.Length, messagesFromReceiver.Length);
			return mapper.Map<MessageViewModel[]>(messages.OrderBy(x=>x.CreatedOn).ToArray());
		}
		[Route("add/{senderId}/receiver/{receiverId}")]
        [HttpPost]
        public async Task<ActionResult> Add(int senderId, int receiverId,[FromBody] string text)
        {
            var message = context.Add(new Message { SenderId = senderId, ReceiverId = receiverId, Text = text, CreatedOn = DateTime.Now });

            context.SaveChanges();
			int firstHubId = Math.Max(senderId, receiverId);
			int secondHubId = Math.Min(senderId, receiverId);
			await hubContext.Clients.Groups($"chat-{firstHubId}{secondHubId}").SendAsync("MessageSent");
			var messageId = message.Property<int>("Id").CurrentValue;
			return Ok(messageId);
        }
        [Route("remove/{senderId}/receiver/{receiverId}")]
        [HttpDelete]
        public async Task<ActionResult> Remove(int senderId, int receiverId)
        {
            Message message = context.Messages.FirstOrDefault(m=>m.SenderId==senderId && m.ReceiverId==receiverId);
            context.Messages.Remove(message);
            context.SaveChanges();
            return Ok();
        }
    }

}
