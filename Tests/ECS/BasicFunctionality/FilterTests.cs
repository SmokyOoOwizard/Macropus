using Macropus.ECS.Component.Storage.Impl;
using Macropus.ECS.Systems;
using Tests.ECS.BasicFunctionality.Components;
using Tests.ECS.BasicFunctionality.Systems;
using Tests.Utils.Tests.ECS;
using Xunit.Abstractions;

namespace Tests.ECS.BasicFunctionality;

public class FilterTests : TestsWithSystems
{
	public FilterTests(ITestOutputHelper output) : base(output) { }

	public override ASystem[] GetSystems()
	{
		return new ASystem[]
		{
			new FilterTestComponentUpdateSystem()
		};
	}

	[Fact]
	public void FilterTest()
	{
		Fill();

		ExecuteSystems();
	}

	private void Fill()
	{
		var buffer = new ComponentsStorage();

		var counter = 0;

		while ((counter & (1 << 10)) == 0)
		{
			var entity = Guid.NewGuid();

			if ((counter & (1 << 0)) != 0)
				buffer.ReplaceComponent(entity, new TestComponent());

			if ((counter & (1 << 1)) != 0)
				buffer.ReplaceComponent(entity, new TestComponent2());

			if ((counter & (1 << 2)) != 0)
				buffer.ReplaceComponent(entity, new TestComponent3());

			if ((counter & (1 << 3)) != 0)
				buffer.ReplaceComponent(entity, new TestComponent4());

			if ((counter & (1 << 4)) != 0)
				buffer.ReplaceComponent(entity, new TestComponent5());

			if ((counter & (1 << 5)) != 0)
				buffer.ReplaceComponent(entity, new TestComponent6());

			if ((counter & (1 << 6)) != 0)
				buffer.ReplaceComponent(entity, new TestComponent7());

			if ((counter & (1 << 7)) != 0)
				buffer.ReplaceComponent(entity, new TestComponent8());

			if ((counter & (1 << 8)) != 0)
				buffer.ReplaceComponent(entity, new TestComponent9());

			if ((counter & (1 << 9)) != 0)
				buffer.ReplaceComponent(entity, new TestComponent10());

			counter++;
		}

		Context.ApplyBuffer(buffer);
	}
}