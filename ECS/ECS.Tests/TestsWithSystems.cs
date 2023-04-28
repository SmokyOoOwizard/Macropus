using Tests.Utils;
using Xunit.Abstractions;

namespace ECS.Tests;

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