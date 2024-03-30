using System.Security.Claims;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class ClaimPrincipleExtensions
	{
		public static string? Id(this ClaimsPrincipal user) => user.FindFirstValue(ClaimTypes.NameIdentifier);
	}
}
