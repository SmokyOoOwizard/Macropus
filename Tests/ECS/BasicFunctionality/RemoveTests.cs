using Macropus.ECS.Component.Exceptions;
using Macropus.ECS.Component.Storage.Impl;
using Macropus.ECS.Systems;
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
		
		var buffer = new ComponentsStorage();
		buffer.ReplaceComponent(entityId, new TestComponent());
		Context.ApplyBuffer(buffer);

		Assert.True(Context.GetHotComponentsStorage().HasComponent<TestComponent>(entityId));

		ExecuteSystems();

		Assert.False(Context.GetHotComponentsStorage().HasComponent<TestComponent>(entityId));
	}

	[Fact]
	public void RemoveNotExistsComponentTest()
	{
		var entityId = Guid.NewGuid();

		Assert.False(Context.GetHotComponentsStorage().HasComponent<TestComponent>(entityId));

		ExecuteSystems();

		Assert.False(Context.GetHotComponentsStorage().HasComponent<TestComponent>(entityId));
	}

	[Fact]
	public void TryRemoveReadOnlyComponentTest()
	{
		var entityId = Guid.NewGuid();
		
		var buffer = new ComponentsStorage();
		buffer.ReplaceComponent(entityId, new ReadOnlyComponent());
		Context.ApplyBuffer(buffer);

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