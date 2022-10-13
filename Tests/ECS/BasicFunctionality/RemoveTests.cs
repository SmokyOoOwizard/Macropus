using Macropus.ECS.Component.Exceptions;
using Macropus.ECS.System;
using Tests.ECS.BasicFunctionality.Components;
using Tests.ECS.BasicFunctionality.Systems;
using Tests.Utils.Tests.ECS;
using Xunit.Abstractions;

namespace Tests.ECS.BasicFunctionality;

public class RemoveTests : TestsWithSystems
{
	public RemoveTests(ITestOutputHelper output) : base(output) { }

	public override ASystem[] GetSystems()
	{
		return new ASystem[]
		{
			new RemoveTestComponentSystem(),
			new RemoveReadOnlyComponentSystem()
		};
	}

	[Fact]
	public void RemoveExistsComponentTest()
	{
		var entityId = Guid.NewGuid();
		NewComponents.ReplaceComponent(entityId, new TestComponent());

		Assert.True(NewComponents.HasComponent<TestComponent>(entityId));

		ExecuteSystems();

		Assert.False(ChangedComponents.HasComponent<TestComponent>(entityId));
		Assert.True(NewComponents.HasComponent<TestComponent>(entityId));
	}

	[Fact]
	public void RemoveNotExistsComponentTest()
	{
		var entityId = Guid.NewGuid();

		Assert.False(NewComponents.HasComponent<TestComponent>(entityId));

		ExecuteSystems();

		Assert.False(ChangedComponents.HasComponent<TestComponent>(entityId));
		Assert.False(NewComponents.HasComponent<TestComponent>(entityId));
	}

	[Fact]
	public void TryRemoveReadOnlyComponentTest()
	{
		var entityId = Guid.NewGuid();
		NewComponents.ReplaceComponent(entityId, new ReadOnlyComponent());

		try
		{
			ExecuteSystems();
		}
		catch (Exception e)
		{
			if (e is not IsReadOnlyComponentException) throw;
		}
	}
}