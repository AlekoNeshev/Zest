using Microsoft.AspNetCore.Mvc;
using Zest.Services.Infrastructure.Interfaces;

namespace Zest.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	
	public class SignalRGroupsController : ControllerBase
	{
       
		private readonly ISignaRService _signalRService;
		public SignalRGroupsController(ISignaRService signalRService)

		{
			_signalRService = signalRService;
          
        }
		
        [HttpPost]
		[Route("addConnectionToGroup/{connectionId}")]
		public async Task<ActionResult> AddConnectionToGroup(string connectionId, [FromBody]string[]? groupsId)
		{
			await _signalRService.AddConnectionToGroup(connectionId, groupsId);
			return Ok();
		
		}
		[HttpPost]
		[Route("removeConnectionToGroup/{connectionId}")]
		public async Task<ActionResult> RemoveConnectionFromAllGroups(string connectionId)
		{
			await _signalRService.RemoveConnectionFromAllGroups(connectionId);
			return Ok();
		}

	}
}
