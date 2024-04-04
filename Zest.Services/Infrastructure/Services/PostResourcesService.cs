using AutoMapper;
using HeyRed.Mime;
using Microsoft.AspNetCore.Http;
using Zest.DBModels;
using Zest.DBModels.Models;
using Zest.Services.ActionResult;
using Zest.Services.Infrastructure.Interfaces;
using Zest.ViewModels.ViewModels;

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


				var type = MimeTypesMap.GetMimeType(postedFile.FileName);
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
				var uploads = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads");

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

		public async Task<CustomFileStreamResult?> GetFileAsync(string fileName)
		{
			var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			var uploads = Path.Combine(baseDirectory, "uploads");
			var filePath = Path.Combine(uploads, fileName);
			
			var mimeType = MimeTypesMap.GetMimeType(fileName);
			var fileStream = System.IO.File.OpenRead(filePath);

			
			return new CustomFileStreamResult(fileStream, mimeType);
		}

		public async Task<PostRescourcesViewModel[]> GetPostResourcesByPostIdAsync(int postId)
		{
			List<PostRescourcesViewModel> fileResults = new List<PostRescourcesViewModel>();
			var uploads = _context.PostResources.Where(x => x.PostId == postId).ToArray();
			
			foreach (var x in uploads)
			{
				PostRescourcesViewModel postRescourcesViewModel = new PostRescourcesViewModel();
				fileResults.Add(new PostRescourcesViewModel()
				{
					Id = x.Id,
					Type = x.Type,
					Source = "https://jwz46sp0-5132.euw.devtunnels.ms/Zest/PostResources/get/"+x.Name
				});
			}
			return fileResults.ToArray();
		}

		
	}

}
