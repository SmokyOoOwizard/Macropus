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
		Assert.True(group.Count == 1);

		ExecuteSystems();
		Context.SaveChanges();

		Assert.True(group.Count == 0);
	}

	[Fact]
	public void RemoveNotExistsComponent()
	{
		var entityId = Guid.NewGuid();

		var buffer = new ComponentsStorage();
		buffer.ReplaceComponent(entityId, new EmptyTestComponent2());
		Context.ApplyChanges(buffer);
		Context.SaveChanges();

		var group = Context.GetGroup(ComponentsFilter.AllOf(typeof(EmptyTestComponent1)).Build());
		Assert.True(group.Count == 0);

		ExecuteSystems();

		Assert.True(group.Count == 0);
	}
}