using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;
using Zest.ViewModels.ViewModels;
using Microsoft.AspNetCore.Http;
using Zest.Services.ActionResult;
namespace Zest.Services.Infrastructure.Interfaces
{
	public interface IPostResourcesService
	{
		Task<PostResources> AddPostResourceAsync(PostResources postResource);
		Task<string> UploadFileAsync(int postId, IFormFileCollection postedFile);
		Task<CustomFileStreamResult?> GetFileAsync(string fileName);
		Task<PostRescourcesViewModel[]> GetPostResourcesByPostIdAsync(int postId);
	}
}
