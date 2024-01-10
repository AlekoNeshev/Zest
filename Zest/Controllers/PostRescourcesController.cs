using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.RegularExpressions;
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

		[HttpPost("ivan")]
		public async Task<IActionResult> UploadFile(IFormFile postedFile)
		{
			//if (!string.Equals(postedFile.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
			//   !string.Equals(postedFile.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
			//   !string.Equals(postedFile.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
			//   !string.Equals(postedFile.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
			//   !string.Equals(postedFile.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
			//   !string.Equals(postedFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
			//{
			//	return Problem();
			//}

			////-------------------------------------------
			////  Check the image extension
			////-------------------------------------------
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
			}

			return Ok($"{un}{postedFileExtension}");
		}

		[HttpGet("ivan/{fileName}")]
		public async Task<FileResult> GetFile([FromQuery] string fileName)
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

	}
}
