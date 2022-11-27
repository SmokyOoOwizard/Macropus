using Delogger.Scope;
using Delogger.Scope.Log;
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.WebApi;
using Macropus.Service;
using Swan.Logging;

namespace Macropus.WebApi;

public class WebApiService : IService
{
	private readonly IDScope dScope;
	private readonly IDLogger dLogger;

	private WebServer server;

	public EServiceStatus Status { get; private set; } = EServiceStatus.ReadyToStart;


	public WebApiService(IDScope dScope)
	{
		this.dScope = dScope;
		dLogger = dScope.CreateLogger(new LoggerCreateOptions() { Tags = new[] { nameof(WebApiService) } });

		// TODO write dLogger wrapper for SwanLogger
		Logger.UnregisterLogger<ConsoleLogger>();
	}


	public void Start()
	{
		Status = EServiceStatus.Starting;

		server = new WebServer(o => o
				.WithUrlPrefix("http://localhost:9696/")
				.WithMode(HttpListenerMode.Microsoft))
			// TODO write autofac wrapper for pass controllers into EmbedIo
			.WithWebApi("/api", m => { m.WithController<PeopleController>(); })
			.WithModule(new ActionModule("/", HttpVerbs.Any, ctx => ctx.SendDataAsync(new { Message = "Error" })));

		server.StateChanged += (s, e)
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