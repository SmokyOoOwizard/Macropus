using Autofac;
using Macropus.ECS;
using Macropus.ECS.Component.Storage;
using Macropus.ECS.Component.Storage.Impl;
using Macropus.ECS.Context.Impl;
using Macropus.ECS.Impl;
using Tests.Utils;
using Xunit.Abstractions;

namespace ECS.Tests;

public abstract class TestsWithSystems : TestsWrapper
{
	private readonly ECSContext executor;

	public readonly IComponentsStorage ExistsComponents = new ComponentsStorageInMemory();
	public readonly EntityContext Context;

	public TestsWithSystems(ITestOutputHelper output) : base(output)
	{
		executor = Container.Resolve<ECSContext>();
		Context = Container.Resolve<EntityContext>();
	}

	protected override void Configure(ContainerBuilder builder)
	{
		base.Configure(builder);
		builder.RegisterInstance<EntityContext>(new("SomeContext", ExistsComponents))
			.AsSelf()
			.AsImplementedInterfaces()
			.SingleInstance();
		builder.RegisterType<ECSContext>()
			.SingleInstance();
	}

	// ReSharper disable once InconsistentNaming
	public void ExecuteSystems()
	{
		executor.Tick();
	}
}