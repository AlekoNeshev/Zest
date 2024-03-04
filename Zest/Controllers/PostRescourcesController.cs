using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.ViewModels.ViewModels;

namespace Zest.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class PostRescourcesController : Controller
	{
		private ZestContext context;
		private IMapper mapper;
        public PostRescourcesController(ZestContext zestContext, IMapper mapper)
        {
            this.context = zestContext;
			this.mapper = mapper;
        }
		
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

		[HttpPost("ivan/{postId}")]
		public async Task<IActionResult> UploadFile(int postId, IFormFile postedFile)
		{
			/*if (!string.Equals(postedFile.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
			   !string.Equals(postedFile.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
			   !string.Equals(postedFile.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
			   !string.Equals(postedFile.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
			   !string.Equals(postedFile.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
			   !string.Equals(postedFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase) && 
			   )
			{
				return Problem();
			}*/


			var postedFileExtension = Path.GetExtension(postedFile.FileName);
			//if (!string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
			//	&& !string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
			//	&& !string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
			//	&& !string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
			//{
			//	return Problem();
			//}

			string uploads = Path.Combine(Assembly.GetEntryAssembly().Location.Replace("Zest.dll", ""), "uploads");
			if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
			
			var un = Guid.NewGuid().ToString();
			
			if (postedFile.Length > 0)
			{
				string filePath = Path.Combine(uploads, $"{un}{postedFileExtension}");
				using (Stream fileStream = new FileStream(filePath, FileMode.Create))
				{
					await postedFile.CopyToAsync(fileStream);
				}
				context.Add(new PostResources { Name = $"{un}{postedFileExtension}", Type = GetMimeTypeByWindowsRegistry(postedFile.FileName).Split("/").ToArray()[0].Trim(), Path = filePath, CreatedOn = DateTime.Now, PostId = postId });
				await context.SaveChangesAsync();
			}

			return Ok($"{un}{postedFileExtension}");
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
			List<PostRescourcesViewModel> fileResults = new List<PostRescourcesViewModel>();
			var uploads = (context.PostResources.Where(x => x.PostId == postId).ToArray());
			foreach (var x in uploads)
			{
				PostRescourcesViewModel postRescourcesViewModel = new PostRescourcesViewModel();
				fileResults.Add(new PostRescourcesViewModel()
				{
					Id = x.PostId,
					Type = x.Type,
					Source = "https://localhost:7183/api/PostRescources/ivan/"+x.Name
				});
			}
			return fileResults.ToArray();
		}
	}
}
