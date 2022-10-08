using Macropus.ECS;
using Macropus.ECS.ComponentsStorage;
using Macropus.ECS.ComponentsStorage.Impl;
using Macropus.ECS.System;
using Xunit.Abstractions;

namespace Tests.Utils.ECS;

public abstract class TestsWithSystems : TestsWrapper
{
	private readonly SystemsExecutor executor;

	public readonly IClearableComponentsStorage ComponentsStorage;
	public readonly IClearableComponentsStorage ChangesComponents;

	public TestsWithSystems(ITestOutputHelper output) : base(output)
	{
		ComponentsStorage = new ComponentsStorage();
		ChangesComponents = new ComponentsStorage();

		// ReSharper disable once VirtualMemberCallInConstructor
		executor = new SystemsExecutor(GetSystems());
	}

	public abstract ASystem[] GetSystems();

	// ReSharper disable once InconsistentNaming
	public void ExecuteSystems()
	{
		executor.Execute(
			ComponentsStorage,
			ComponentsStorage.GetEnumerationComponents(true),
			ChangesComponents);
	}


	[Fact]
	public void EmptyExecuteTest()
	{
		ExecuteSystems();
	}
}