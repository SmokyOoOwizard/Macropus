using Macropus.ECS.Component.Trigger;
using Macropus.ECS.Systems.Exceptions;
using Tests.Utils;
using Xunit.Abstractions;

namespace ECS.Tests;

public class BrokenTriggerTest : TestsWrapper
{
	public BrokenTriggerTest(ITestOutputHelper output) : base(output) { }

	[Fact]
	public void BuildTriggerWithWrongTypes()
	{
		try
		{
			ComponentsTrigger.AllOf(typeof(string)).Build();
		}
		catch (Exception e)
		{
			if (e is not TypesAreNotComponentsException)
				throw;

			return;
		}

		Assert.Fail("Components trigger must throw exception if types if not components");
	}
}