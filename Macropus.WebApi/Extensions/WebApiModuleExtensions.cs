using System.Reflection;
using EmbedIO;
using EmbedIO.WebApi;

namespace Macropus.WebApi.Extensions;

public static class WebApiModuleExtensions
{
	private static readonly Type FuncWebApiControllerType = typeof(Func<WebApiController>);

	private static readonly MethodInfo ResolveControllerMethodInfo
		= typeof(AWebApiModule).GetMethod(nameof(AWebApiModule.ResolveController))!;

	public static void SetupModule(this AWebApiModule module, WebServer server, string url)
	{
		var webModule = new WebApiModule(url);

		var types = module.GetWebApiControllers();

		var resolveGeneric = ResolveControllerMethodInfo;

		foreach (var type in types)
		{
			var resolveInfo = resolveGeneric.MakeGenericMethod(type);

			var resolve = Delegate.CreateDelegate(FuncWebApiControllerType, module, resolveInfo);

			webModule.WithController(type, (Func<WebApiController>)resolve);
		}

		server.WithModule(webModule);
	}
}