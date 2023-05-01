using Autofac;
using ECS.Tests.Trigger.Components;
using ECS.Tests.Trigger.Systems;
using Macropus.ECS.Component.Filter;
using Macropus.ECS.Component.Storage.Impl;
using Macropus.ECS.Systems;
using Xunit.Abstractions;

namespace ECS.Tests.Trigger;

public class NoneOfTriggerTests : TestsWithSystems
{
	public NoneOfTriggerTests(ITestOutputHelper output) : base(output) { }

	protected override void Configure(ContainerBuilder builder)
	{
		base.Configure(builder);
		builder.RegisterType<FewNoneOfTriggerSystem>()
			.AsImplementedInterfaces()
			.AsSelf()
			.As<ISystem>()
			.SingleInstance();
	}

	[Fact]
	public void RemoveComponent()
	{
		var buffer = new ComponentsStorage();
		var entityId = Guid.NewGuid();
		buffer.ReplaceComponent(entityId, new EmptyTestComponent1());
		buffer.ReplaceComponent(entityId, new EmptyTestComponent2());
		buffer.ReplaceComponent(entityId, new EmptyTestComponent3());

		entityId = Guid.NewGuid();
		buffer.ReplaceComponent(entityId, new EmptyTestComponent3());

		Context.ApplyChanges(buffer);
		Context.SaveChanges();

		var group = Context.GetGroup(ComponentsFilter.AllOf(typeof(EmptyTestComponent3)).Build());
		Assert.Equal(2, group.Count);

		ExecuteSystems();

		Assert.Equal(1, group.Count);
		Assert.DoesNotContain(group.AsEnumerable(), entity => entity.Id == entityId);
	}

	[Fact]
	public void RemoveNotExistsComponent()
	{
		var entityId = Guid.NewGuid();

		var buffer = new ComponentsStorage();
		buffer.ReplaceComponent(entityId, new EmptyTestComponent4());
		Context.ApplyChanges(buffer);
		Context.SaveChanges();

		var group = Context.GetGroup(ComponentsFilter.AllOf(typeof(EmptyTestComponent4)).Build());
		Assert.Equal(1, group.Count);

		ExecuteSystems();

		Assert.Equal(1, group.Count);
	}
}