using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityController : ControllerBase
    {
        private ZestContext context;
        private IMapper mapper;
        public CommunityController(ZestContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult<CommunityViewModel>> Find(int id)
        {
            return mapper.Map<CommunityViewModel>(context.Communities.Where(x => x.Id == id).FirstOrDefault());
        }
        [Route("getAll")]
        [HttpGet]
        public async Task<ActionResult<CommunityViewModel[]>> GetAll()
        {
            return mapper.Map<CommunityViewModel[]>(context.Communities.ToArray());
        }
        [Route("add/{name}/creator/{creatorId}")]
        [HttpPost]
        public async Task<ActionResult> Add(string name, [FromBody] string discription, int creatorId)
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
