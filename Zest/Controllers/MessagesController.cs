﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zest.DBModels;
using Zest.DBModels.Models;

namespace Zest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private ZestContext context;

        public MessagesController(ZestContext context)
        {
            this.context=context;
        }

        [HttpPost]
        public async Task<ActionResult> Add(int senderId, int receiverId, string text)
        {
            context.Add(new Message { SenderId = senderId, ReceiverId = receiverId, Text = text, CreatedOn = DateTime.Now });
            context.SaveChanges();
            return Ok();
        }
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
