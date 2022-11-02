using System.Reflection;
using Xunit.Abstractions;

namespace Tests.Utils.Tests;

public abstract class TestsWrapper
{
	public readonly string ExecutePath;
	
	protected readonly ITestOutputHelper Output;
	
	public TestsWrapper(ITestOutputHelper output)
	{
		Output = output;
		
		var type = output.GetType();
		var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
		var test = (ITest)testMember?.GetValue(output)!;

		var testPrefix = test.TestCase.TestMethod.TestClass.Class.Name?.Replace('.', '\\') ?? string.Empty;

		ExecutePath = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory, "TestArtifacts", testPrefix,
			test.DisplayName, Guid.NewGuid().ToString("N"));
		
		
		var converter = new ConsoleConverter(output, Path.Combine(ExecutePath, "log.txt"));
		Console.SetOut(converter);
	}
}