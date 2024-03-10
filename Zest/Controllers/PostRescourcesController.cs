using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	
	public class PostRescourcesController : Controller
	{
		private ZestContext context;
		private IMapper mapper;
		private IPostResourcesService _postResourceService;
        public PostRescourcesController(ZestContext zestContext, IMapper mapper, IPostResourcesService postResourcesService)
        {
            this.context = zestContext;
			this.mapper = mapper;
			this._postResourceService = postResourcesService;
        }
		

		[HttpPost("uploadFile/{postId}")]
		public async Task<IActionResult> UploadFile(int postId, IFormFile postedFile)
		{
			var file = await _postResourceService.UploadFileAsync(postId, postedFile);

			return Ok(file);
		}

		[HttpGet("ivan/{fileName}")]
		public async Task<FileResult> GetFile(string fileName)
		{
			string uploads = Path.Combine(Assembly.GetEntryAssembly().Location.Replace("Zest.dll", ""), "uploads");
			return File(System.IO.File.OpenRead(Path.Combine(uploads, fileName)), GetMimeTypeByWindowsRegistry(fileName.Split(".").Last()));
		}

		public static string GetMimeTypeByWindowsRegistry(string fileNameOrExtension)
		{
			string mimeType = "application/unknown";
			string ext = (fileNameOrExtension.Contains(".")) ? System.IO.Path.GetExtension(fileNameOrExtension).ToLower() : "." + fileNameOrExtension;
			Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
			if (regKey != null && regKey.GetValue("Content Type") != null) mimeType = regKey.GetValue("Content Type").ToString();
			return mimeType;
		}
		[HttpGet("getByPostId/{postId}")]
		public async Task<PostRescourcesViewModel[]> GetPhotos(int postId)
		{
			var fileResults = await _postResourceService.GetPostResourcesByPostIdAsync(postId);
			return fileResults;
		}
	}
}
