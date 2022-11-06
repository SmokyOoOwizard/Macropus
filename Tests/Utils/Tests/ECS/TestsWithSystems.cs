using Macropus.ECS;
using Macropus.ECS.ComponentsStorage;
using Macropus.ECS.ComponentsStorage.Impl;
using Macropus.ECS.Systems;
using Xunit.Abstractions;

namespace Tests.Utils.Tests.ECS;

public abstract class TestsWithSystems : TestsWrapper
{
	private readonly SystemsExecutor executor;

	public readonly IComponentsStorage AlreadyExistsComponents;
	public readonly IComponentsStorage NewComponents;
	public readonly IComponentsStorage ChangedComponents;

	public TestsWithSystems(ITestOutputHelper output) : base(output)
	{
		AlreadyExistsComponents = new ComponentsStorage();
		NewComponents = new ComponentsStorage();
		ChangedComponents = new ComponentsStorage();

		// ReSharper disable once VirtualMemberCallInConstructor
		executor = new SystemsExecutor(GetSystems());
	}

	public abstract ASystem[] GetSystems();

	// ReSharper disable once InconsistentNaming
	public void ExecuteSystems()
	{
		executor.Execute(
			AlreadyExistsComponents,
			NewComponents,
			ChangedComponents);
	}

	[Fact]
	public void EmptyExecuteTest()
	{
		ExecuteSystems();
	}
}