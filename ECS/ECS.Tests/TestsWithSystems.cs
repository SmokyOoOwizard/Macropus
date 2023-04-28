using Autofac;
using Macropus.ECS;
using Macropus.ECS.Component.Storage;
using Macropus.ECS.Component.Storage.Impl;
using Macropus.ECS.Entity.Context;
using Tests.Utils;
using Xunit.Abstractions;

namespace ECS.Tests;

public abstract class TestsWithSystems : TestsWrapper
{
	private readonly SystemsExecutor executor;

	public readonly IComponentsStorage ExistsComponents;
	public readonly EntitiesContext Context;

	public TestsWithSystems(ITestOutputHelper output) : base(output)
	{
		ExistsComponents = new ComponentsStorage();

		Context = new(ExistsComponents);

		executor = Mock.Create<SystemsExecutor>();

		executor.SetCollectors(Context);
	}

	protected override void Configure(ContainerBuilder builder)
	{
		base.Configure(builder);
		builder.RegisterType<SystemsExecutor>().SingleInstance();
	}

	// ReSharper disable once InconsistentNaming
	public void ExecuteSystems()
	{
		executor.Execute(Context);
	}
}