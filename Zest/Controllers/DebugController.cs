using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Zest.Controllers;

[Route("debug")]
public class DebugController : Controller
{

	[Authorize]
	[HttpGet]
	public IActionResult Index()
	{
		return Ok(User.Identity);
	}

}
