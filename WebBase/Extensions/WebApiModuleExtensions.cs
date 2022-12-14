using System.Reflection;
using EmbedIO;
using EmbedIO.WebApi;

namespace Macropus.Web.Base.Extensions;

public static class WebApiModuleExtensions
{
	private static readonly Type FuncWebApiControllerType = typeof(Func<WebApiController>);

	private static readonly MethodInfo ResolveMethodInfo
		= typeof(AWebModule).GetMethod(nameof(AWebModule.Resolve))!;


	public static void SetupModule(this AWebModule module, WebServer server)
	{
		SetupModule(module, server, module.Url);
	}

	public static void SetupModule(this AWebModule module, WebServer server, string url)
	{
		var webModule = new WebApiModule(url);

		var types = module.GetWebApiControllers();

		var resolveGeneric = ResolveMethodInfo;

		foreach (var type in types)
		{
			var resolveInfo = resolveGeneric.MakeGenericMethod(type);

			var resolve = Delegate.CreateDelegate(FuncWebApiControllerType, module, resolveInfo);

			webModule.WithController(type, (Func<WebApiController>)resolve);
		}

		server.WithModule(webModule);
	}
}