﻿using Autofac;
using Delogger.Scope;
using Delogger.Scope.Log;
using EmbedIO;
using Macropus.Service;
using Macropus.Web.Base.Wrapper;
using Macropus.Web.Base.Extensions;
using Swan.Logging;

namespace Macropus.Web.Base.Service;

public class WebService : IService
{
	private readonly ILifetimeScope scope;
	private readonly IDScope dScope;
	private readonly IDLogger logger;

	private readonly WebServer server;

	public EServiceStatus Status { get; private set; } = EServiceStatus.ReadyToStart;


	public WebService(ILifetimeScope scope, IDScope dScope)
	{
		this.scope = scope;
		this.dScope = dScope;
		logger = dScope.CreateLogger(new LoggerCreateOptions { Tags = new[] { nameof(WebService) } });

		Logger.NoLogging();
		Logger.RegisterLogger(new DeloggerWrapperForSwanILogger(dScope.CreateLogger(new LoggerCreateOptions
		{
			Tags = new[] { nameof(WebService) }
		})));

		server = new WebServer(ConfigureServer);
	}

	private void ConfigureServer(WebServerOptions options)
	{
		options.WithUrlPrefix("http://localhost:9696/");
		options.WithMode(HttpListenerMode.Microsoft);
	}

	private void SetupEndpoints()
	{
		foreach (var module in scope.Resolve<IEnumerable<AWebModule>>())
		{
			module.SetupModule(server);

			var endpoints = module.GetEndpoints()
				.Select(kv
					=> new KeyValuePair<string, object>(kv.Key.Name, $"{kv.Value.Verb} {kv.Value.Route}"))
				.ToArray();

			if (endpoints.Length > 0)
			{
				logger.Log("Setup endpoints {0} {1}",
					new[] { "Endpoints", "Setup" },
					new object[] { module.GetType().Name, module.Url },
					endpoints
				);
			}
		}
	}

	private void SetupModules()
	{
		foreach (var module in scope.Resolve<IEnumerable<IWebModule>>())
		{
			server.WithModule(module);
			logger.Log("Setup module {0}",
				new[] { "Module", "Setup" },
				new object[] { module.GetType().Name }
			);
		}
	}


	public void Start()
	{
		Status = EServiceStatus.Starting;

		SetupEndpoints();
		SetupModules();

		server.StateChanged += (_, e)
			=> logger.Log("Web server state: {0}", null, new object[] { e.NewState });

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