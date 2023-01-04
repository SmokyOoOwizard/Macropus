using System.Globalization;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;

namespace Macropus.Web.Api;

public class TimeController : WebApiController
{
	[Route(HttpVerbs.Get, "/time")]
	public object Get() => new { Time = DateTime.Now.ToString("MM.dd.yyyy HH:mm:ss:ffff") };
}