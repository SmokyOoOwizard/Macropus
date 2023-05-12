using Autofac;
using ECS.Tests.Filter.Components;
using ECS.Tests.Remove.Systems;
using Macropus.ECS.Component.Filter;
using Macropus.ECS.Component.Storage.Impl;
using Macropus.ECS.Systems;
using Xunit.Abstractions;

namespace ECS.Tests.Remove;

public class RemoveComponentTests : TestsWithSystems
{
	public RemoveComponentTests(ITestOutputHelper output) : base(output) { }

	protected override void Configure(ContainerBuilder builder)
	{
		base.Configure(builder);
		builder.RegisterType<RemoveTestComponentSystem>()
			.AsImplementedInterfaces()
			.AsSelf()
			.As<ISystem>()
			.SingleInstance();
	}

	[Fact]
	public void RemoveComponent()
	{
		var entityId = Guid.NewGuid();

		var buffer = new ComponentsStorage();
		buffer.ReplaceComponent(entityId, new EmptyTestComponent1());
		Context.ApplyChanges(buffer);
		Context.SaveChanges();

		var group = Context.GetGroup(ComponentsFilter.AllOf(typeof(EmptyTestComponent1)).Build());
		Assert.Equal(1, group.Count);

		ExecuteSystems();
		Context.SaveChanges();

		Assert.Equal(0, group.Count);
	}

	[Fact]
	public void RemoveNotExistsComponent()
	{
		var entityId = Guid.NewGuid();

		var buffer = new ComponentsStorage();
		buffer.ReplaceComponent(entityId, new EmptyTestComponent2());
		Context.ApplyChanges(buffer);
		Context.SaveChanges();

		var group = Context.GetGroup(ComponentsFilter.AllOf(typeof(EmptyTestComponent2)).Build());
		Assert.Equal(1, group.Count);

		ExecuteSystems();

		Assert.Equal(1, group.Count);
	}
}