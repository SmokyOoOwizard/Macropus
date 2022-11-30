using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;

namespace Macropus.Web.Base.Controllers;

public sealed class NotFoundController : WebApiController
{
	[Route(HttpVerbs.Any, "/", true)]
	public void NotFound()
	{
		throw HttpException.NotFound();
	}
}