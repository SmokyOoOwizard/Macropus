using Macropus.ECS.System;
using Tests.ECS.BasicFunctionality.Components;
using Tests.ECS.BasicFunctionality.Systems;
using Tests.Utils.ECS;
using Xunit.Abstractions;

namespace Tests.ECS.BasicFunctionality;

public class FilterTests : TestsWithSystems
{
	public FilterTests(ITestOutputHelper output) : base(output) { }

	public override ASystem[] GetSystems()
	{
		return new ASystem[]
		{
			new FilterTestComponentSystem()
		};
	}

	[Fact]
	public void FilterTest()
	{
		var entityId = Guid.NewGuid();
		NewComponents.ReplaceComponent(entityId, new TestComponent());

		var entityId2 = Guid.NewGuid();
		NewComponents.ReplaceComponent(entityId2, new ReadOnlyComponent());

		ExecuteSystems();
	}
}