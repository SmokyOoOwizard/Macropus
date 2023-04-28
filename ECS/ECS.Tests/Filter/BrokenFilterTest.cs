using Macropus.ECS.Component.Filter;
using Macropus.ECS.Systems.Exceptions;
using Tests.Utils;
using Xunit.Abstractions;

namespace ECS.Tests.Filter;

public class BrokenFilterTest : TestsWrapper
{
	public BrokenFilterTest(ITestOutputHelper output) : base(output) { }

	[Fact]
	public void BuildFilterWithWrongTypes()
	{
		try
		{
			ComponentsFilter.AllOf(typeof(string)).Build();
		}
		catch (Exception e)
		{
			if (e is not TypesAreNotComponentsException)
				throw;

			return;
		}

		Assert.Fail("Components filter must throw exception if types if not components");
	}
}