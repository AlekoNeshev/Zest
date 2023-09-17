using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Zest.DBModels;
using Zest.DBModels.Models;

namespace Zest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityController : ControllerBase
    {
        private ZestContext context;
        public CommunityController(ZestContext context)
        {
            this.context = context;
        }

        public async Task<ActionResult<Community>> Find(int id)
        {
            return context.Communities.Where(x=>x.Id == id).FirstOrDefault();
        }

        [HttpPost]
        public async Task<ActionResult> Add(string name, string discription, int creatorId)
        {
            context.Add(new Community
            {
                Name = name,
                Information = discription,
                CreatorId = creatorId,
                CreatedOn = DateTime.Now,
            });
            context.SaveChanges();
            return Ok();
        }
    }
}
