using Autofac;
using ECS.Tests.Filter.Components;
using ECS.Tests.Remove.Systems;
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
			.As<ASystem>()
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

		Assert.True(Context.cold.HasComponent<EmptyTestComponent1>(entityId));

		ExecuteSystems();
		Context.SaveChanges();

		Assert.False(Context.cold.HasComponent<EmptyTestComponent1>(entityId));
	}

	[Fact]
	public void RemoveNotExistsComponent()
	{
		var entityId = Guid.NewGuid();

		Assert.False(Context.cold.HasComponent<EmptyTestComponent1>(entityId));

		ExecuteSystems();

		Assert.False(Context.cold.HasComponent<EmptyTestComponent1>(entityId));
	}
}