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

	public readonly IComponentsStorage ExistsComponents = new ComponentsStorage();
	public readonly EntityContext Context;

	public TestsWithSystems(ITestOutputHelper output) : base(output)
	{
		executor = Container.Resolve<SystemsExecutor>();
		Context = Container.Resolve<EntityContext>();

		executor.SetContext(Context);
	}

	protected override void Configure(ContainerBuilder builder)
	{
		base.Configure(builder);
		builder.RegisterInstance<EntityContext>(new("SomeContext", ExistsComponents))
			.AsSelf()
			.AsImplementedInterfaces();
		builder.RegisterType<SystemsExecutor>().SingleInstance();
	}

	// ReSharper disable once InconsistentNaming
	public void ExecuteSystems()
	{
		executor.Execute(Context);
	}
}