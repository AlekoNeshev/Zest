using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Zest.DBModels;
using Zest.Services.ActionResult;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	
	public class PostRescourcesController : Controller
	{
		
		private IPostResourcesService _postResourceService;
        public PostRescourcesController(IPostResourcesService postResourcesService)
        {
           
			this._postResourceService = postResourcesService;
        }
		

		[HttpPost("uploadFile/{postId}")]
		public async Task<IActionResult> UploadFile(int postId, IFormFileCollection postedFiles)
		{
			
			 await _postResourceService.UploadFileAsync(postId, postedFiles);

			return Ok();
		}

		[HttpGet("ivan/{fileName}")]
		public async Task<FileResult> GetFile(string fileName)
		{
			CustomFileStreamResult customFileStreamResult = await _postResourceService.GetFileAsync(fileName);
			
			return File(customFileStreamResult.Stream, customFileStreamResult.ContentType);
		}

		
		[HttpGet("getByPostId/{postId}")]
		public async Task<PostRescourcesViewModel[]> GetPhotos(int postId)
		{
			var fileResults = await _postResourceService.GetPostResourcesByPostIdAsync(postId);
			return fileResults;
		}
	}
}
