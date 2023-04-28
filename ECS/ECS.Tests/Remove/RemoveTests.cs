using Autofac;
using ECS.Tests.Remove.Components;
using ECS.Tests.Remove.Systems;
using Macropus.ECS.Component.Exceptions;
using Macropus.ECS.Component.Storage.Impl;
using Macropus.ECS.Systems;
using Xunit.Abstractions;

namespace ECS.Tests.Remove;

public class RemoveReadOnlyComponentTest : TestsWithSystems
{
	public RemoveReadOnlyComponentTest(ITestOutputHelper output) : base(output) { }

	protected override void Configure(ContainerBuilder builder)
	{
		base.Configure(builder);
		builder.RegisterType<RemoveReadOnlyComponentSystem>()
			.AsImplementedInterfaces()
			.AsSelf()
			.As<ASystem>()
			.SingleInstance();
	}

	[Fact]
	public void RemoveReadOnlyComponent()
	{
		var entityId = Guid.NewGuid();

		var buffer = new ComponentsStorage();
		buffer.ReplaceComponent(entityId, new ReadOnlyComponent());
		Context.ApplyChanges(buffer);

		try
		{
			ExecuteSystems();
		}
		catch (Exception e)
		{
			if (e is IsReadOnlyComponentException)
				return;

			throw;
		}

		Assert.Fail("Context must throw exception if we try remove readonly component");
	}
}