﻿using Microsoft.AspNetCore.Mvc;
using Zest.Services.ActionResult;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
	[Route("Zest/[controller]")]
	[ApiController]
	
	public class PostResourcesController : Controller
	{
		
		private readonly IPostResourcesService _postResourceService;
		private readonly IPostService _postService;
        public PostResourcesController(IPostResourcesService postResourcesService, IPostService postService)
        {
           
			this._postResourceService = postResourcesService;
			_postService = postService;
        }
		

		[HttpPost("uploadFile/{postId}")]
		public async Task<IActionResult> UploadFile(int postId, IFormFileCollection postedFiles)
		{

			var doesPostExist = await _postService.DoesExist(postId);
			if (!doesPostExist)
			{
				return BadRequest("Post does not exist!");
			}
			await _postResourceService.UploadFileAsync(postId, postedFiles);

			return Ok();
		}

		[HttpGet("get/{fileName}")]
		public async Task<FileResult> GetFile(string fileName)
		{
			CustomFileStreamResult customFileStreamResult = await _postResourceService.GetFileAsync(fileName);
			
			return File(customFileStreamResult.Stream, customFileStreamResult.ContentType);
		}

		
		[HttpGet("getByPostId/{postId}")]
		public async Task<ActionResult<PostRescourcesViewModel[]>> GetPhotos(int postId)
		{
			var doesPostExist = await _postService.DoesExist(postId);
			if (!doesPostExist)
			{
				return BadRequest("Post does not exist!");
			}
			var fileResults = await _postResourceService.GetPostResourcesByPostIdAsync(postId);
			return fileResults;
		}
	}
}
