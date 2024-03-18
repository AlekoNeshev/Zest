using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;
using Zest.DBModels;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Web.Mvc;
using Microsoft.AspNetCore.Http;

namespace Zest.Services.Infrastructure.Services
{
	public class PostResourcesService : IPostResourcesService
	{
		private readonly ZestContext _context;
		private readonly IMapper _mapper;

		public PostResourcesService(ZestContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<PostRescourcesViewModel> GetPostResourceByIdAsync(int id)
		{
			var postResource = await _context.PostResources.FindAsync(id);
			return _mapper.Map<PostRescourcesViewModel>(postResource);
		}

		public async Task<PostResources> AddPostResourceAsync(PostResources postResource)
		{
			_context.PostResources.Add(postResource);
			await _context.SaveChangesAsync();
			return postResource;
		}

		public async Task<string> UploadFileAsync(int postId, IFormFileCollection postedFiles)
		{
			foreach (var postedFile in postedFiles)
			{


				var type = GetMimeTypeByWindowsRegistry(postedFile.FileName);
				if (!string.Equals(type, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
				   !string.Equals(type, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
				   !string.Equals(type, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
				   !string.Equals(type, "image/gif", StringComparison.OrdinalIgnoreCase) &&
				   !string.Equals(type, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
				   !string.Equals(type, "image/png", StringComparison.OrdinalIgnoreCase)&& !string.Equals(type, "video/mp4", StringComparison.OrdinalIgnoreCase))


				{
					return "Incorrect mime!";
				}


				var postedFileExtension = Path.GetExtension(postedFile.FileName);
				if (!string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
					&& !string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
					&& !string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
					&& !string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase)&& !string.Equals(postedFileExtension, ".mp4", StringComparison.OrdinalIgnoreCase))
				{
					return "Incorrect extension!";
				}

				var newName = Guid.NewGuid().ToString();
				var uploads = Path.Combine(Assembly.GetEntryAssembly().Location.Replace("Zest.dll", ""), "uploads");

				if (!Directory.Exists(uploads))
				{
					Directory.CreateDirectory(uploads);
				}

				if (postedFile.Length > 0)
				{
					var filePath = Path.Combine(uploads, $"{newName}{postedFileExtension}");
					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						await postedFile.CopyToAsync(fileStream);
					}

					var postResource = new PostResources
					{
						Name = $"{newName}{postedFileExtension}",
						Type = type.Split("/").ToArray()[0].Trim(),
						Path = filePath,
						CreatedOn = DateTime.Now,
						PostId = postId
					};

					await AddPostResourceAsync(postResource);
					
				}
			}
			return "Shiish";
		}

		public async Task<FileResult> GetFileAsync(string fileName)
		{
			var uploads = Path.Combine(Assembly.GetEntryAssembly().Location.Replace("Zest.dll", ""), "uploads");
			var filePath = Path.Combine(uploads, fileName);
			var mimeType = GetMimeTypeByWindowsRegistry(fileName.Split(".").Last());
			var fileStream = System.IO.File.OpenRead(filePath);
			return new FileStreamResult(fileStream, mimeType);
		}

		public async Task<PostRescourcesViewModel[]> GetPostResourcesByPostIdAsync(int postId)
		{
			List<PostRescourcesViewModel> fileResults = new List<PostRescourcesViewModel>();
			var uploads = (_context.PostResources.Where(x => x.PostId == postId).ToArray());
			foreach (var x in uploads)
			{
				PostRescourcesViewModel postRescourcesViewModel = new PostRescourcesViewModel();
				fileResults.Add(new PostRescourcesViewModel()
				{
					Id = x.Id,
					Type = x.Type,
					Source = "https://926zh759-5132.euw.devtunnels.ms/api/PostRescources/ivan/"+x.Name
				});
			}
			return fileResults.ToArray();
		}

		private static string GetMimeTypeByWindowsRegistry(string fileNameOrExtension)
		{
			string mimeType = "application/unknown";
			string ext = (fileNameOrExtension.Contains(".")) ? System.IO.Path.GetExtension(fileNameOrExtension).ToLower() : "." + fileNameOrExtension;
			Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
			if (regKey != null && regKey.GetValue("Content Type") != null) mimeType = regKey.GetValue("Content Type").ToString();
			return mimeType;
		}
	}

}
