using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;
using Zest.ViewModels.ViewModels;
using System.Web.Mvc;
using Microsoft.AspNetCore.Http;
namespace Zest.Services.Infrastructure.Interfaces
{
	public interface IPostResourcesService
	{
		Task<PostRescourcesViewModel> GetPostResourceByIdAsync(int id);
		Task<PostResources> AddPostResourceAsync(PostResources postResource);
		Task<string> UploadFileAsync(int postId, IFormFile postedFile);
		Task<FileResult> GetFileAsync(string fileName);
		Task<PostRescourcesViewModel[]> GetPostResourcesByPostIdAsync(int postId);
	}
}
