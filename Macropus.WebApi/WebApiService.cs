using Autofac;
using Delogger.Scope;
using Delogger.Scope.Log;
using EmbedIO;
using EmbedIO.Actions;
using Macropus.Service;
using Macropus.WebApi.Extensions;
using Swan.Logging;

namespace Macropus.WebApi;

public class WebApiService : IService
{
	private readonly ILifetimeScope scope;
	private readonly IDScope dScope;
	private readonly IDLogger dLogger;

	private readonly WebServer server;

	public EServiceStatus Status { get; private set; } = EServiceStatus.ReadyToStart;


	public WebApiService(ILifetimeScope scope, IDScope dScope)
	{
		this.scope = scope;
		this.dScope = dScope;
		dLogger = dScope.CreateLogger(new LoggerCreateOptions { Tags = new[] { nameof(WebApiService) } });

		server = new WebServer(ConfigureServer);
		
		// TODO write dLogger wrapper for SwanLogger
		Logger.NoLogging();
	}

	private void ConfigureServer(WebServerOptions options)
	{
		options.WithUrlPrefix("http://localhost:9696/");
		options.WithMode(HttpListenerMode.Microsoft);
	}

	private void SetupEndpoints()
	{
		scope.Resolve<TestApiModule>().SetupModule(server,"/api");
		server.WithModule(new ActionModule("/", HttpVerbs.Any, ctx => ctx.SendDataAsync(new { Message = "404" })));
	}


	public void Start()
	{
		Status = EServiceStatus.Starting;

		SetupEndpoints();

		server.StateChanged += (_, e)
			=> dLogger.Log("WebApi server state: {0}", null, new object[] { e.NewState });

		server.RunAsync();

		Status = EServiceStatus.Started;
	}

	public void Stop()
	{
		Status = EServiceStatus.Termination;

		server.Dispose();

		Status = EServiceStatus.Terminated;
	}
}