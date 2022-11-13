using Macropus.ECS;
using Macropus.ECS.Component.Storage;
using Macropus.ECS.Component.Storage.Impl;
using Macropus.ECS.Entity.Context;
using Macropus.ECS.Systems;
using Xunit.Abstractions;

namespace Tests.Utils.Tests.ECS;

public abstract class TestsWithSystems : TestsWrapper
{
	private readonly SystemsExecutor executor;

	public readonly IComponentsStorage ExistsComponents;
	public readonly EntitiesContext Context;

	public TestsWithSystems(ITestOutputHelper output) : base(output)
	{
		ExistsComponents = new ComponentsStorage();

		Context = new(ExistsComponents);

		// ReSharper disable once VirtualMemberCallInConstructor
		executor = new SystemsExecutor(GetSystems());

		executor.SetCollectors(Context);
	}

	public abstract ASystem[] GetSystems();

	// ReSharper disable once InconsistentNaming
	public void ExecuteSystems()
	{
		executor.Execute(Context);
	}

	[Fact]
	public void EmptyExecuteTest()
	{
		ExecuteSystems();
	}
}