using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PostRescourcesController : Controller
	{
		private ZestContext context;
		private IMapper mapper;
        public PostRescourcesController(ZestContext zestContext, IMapper mapper)
        {
            this.context = zestContext;
			this.mapper = mapper;
        }
		// GET: PostRescourcesController
		[HttpGet]
        public async Task<ActionResult<PostRescourcesViewModel>> Index()
		{

			return mapper.Map<PostRescourcesViewModel>(context.PostResources.Find(1));
		}

		[Route("{type}/path/{path}")]
		[HttpPost]
		public async Task<ActionResult> Add(string type, string path)
		{
			context.Add(new PostResources { Type = type, Path = path, CreatedOn = DateTime.Now, PostId = 1 });
			await context.SaveChangesAsync();
			return Ok();
		}
		
	}
}
