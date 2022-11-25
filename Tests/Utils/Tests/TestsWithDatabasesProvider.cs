﻿using Macropus.Database.Interfaces;
using Macropus.DatabasesProvider;
using Xunit.Abstractions;

#pragma warning disable CS8618

namespace Tests.Utils.Tests;

public abstract class TestsWithDatabasesProvider : TestsWithFileSystemProvider
{
	public IDatabasesService DatabasesService { get; private set; }

	public TestsWithDatabasesProvider(ITestOutputHelper output) : base(output) { }

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync().ConfigureAwait(false);

		DatabasesService = await Macropus.Database.DatabasesService
			.Create(ExecutePath, FileSystemProvider)
			.ConfigureAwait(false);
	}

	public override async Task DisposeAsync()
	{
		DatabasesService.Dispose();

		await base.DisposeAsync();
	}
}