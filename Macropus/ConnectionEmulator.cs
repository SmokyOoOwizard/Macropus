﻿using Autofac;
using Delogger.Scope;
using Macropus.Project.Connection;
using Macropus.Project.Storage;
using Macropus.Project.Storage.Impl;
using Macropus.Service;

namespace Macropus;

public class ConnectionEmulator : IAsyncService
{
	private readonly IDScope logger;
	private readonly IProjectsStorage storage;

	private IProjectConnection? connection;
	
	public EServiceStatus Status { get; private set; } = EServiceStatus.ReadyToStart;

	public ConnectionEmulator(IComponentContext scope, IDScope logger)
	{
		this.logger = logger;
		
		var storageMaster = scope.Resolve<ProjectsStorageMaster>();
		var storageFactory = scope.Resolve<ProjectsStorageLocalFactory>();

		var localStorage = storageFactory.Create(@"D:\TEst");
		storageMaster.AddStorage(localStorage);
		storageMaster.SetDefaultStorage(localStorage);

		storage = storageMaster;
	}

	public async Task StartAsync(CancellationToken cancellationToken = default)
	{
		Status = EServiceStatus.Started;

		var ids = await storage.GetExistsProjectsAsync(null, cancellationToken).ConfigureAwait(false);
		if (ids.Length == 0)
		{
			ids = new[]
			{
				await storage.CreateProjectAsync(new ProjectCreationInfo() { Name = "SSS" }, cancellationToken)
			};
		}


		connection = await storage.OpenProjectAsync(ids[0], null, cancellationToken);
	}

	public Task StopAsync()
	{
		Status = EServiceStatus.Terminated;

		connection?.Dispose();
		return Task.CompletedTask;
	}
}