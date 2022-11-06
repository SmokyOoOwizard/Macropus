using Macropus.ECS;
using Macropus.ECS.Systems.Exceptions;
using Tests.ECS.BasicFunctionality.Systems;
using Tests.Utils;
using Tests.Utils.Tests;
using Xunit.Abstractions;

namespace Tests.ECS.BasicFunctionality;

public class BrokenFilterTests : TestsWrapper
{
	public BrokenFilterTests(ITestOutputHelper output) : base(output) { }

	[Fact]
	public void TryCreateSystemsExecuteWithBrokenFilterTest()
	{
		try
		{
			var systemsExecutor = new SystemsExecutor(new BrokenFilterTestComponentSystem());
		}
		catch (Exception e)
		{
			if (e is not FilterHasTypesWhichAreNotComponentsException)
				throw;
		}
	}
}