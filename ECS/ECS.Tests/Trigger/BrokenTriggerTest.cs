using Macropus.ECS.Component.Trigger;
using Macropus.ECS.Systems.Exceptions;
using Tests.Utils;
using Xunit.Abstractions;

namespace ECS.Tests.Trigger;

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
			if (e is TypesAreNotComponentsException)
				return;

			throw;
		}

		Assert.Fail("Components trigger must throw exception if types if not components");
	}
}