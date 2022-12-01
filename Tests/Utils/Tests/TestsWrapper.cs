using System.Reflection;
using Autofac;
using Autofac.Extras.Moq;
using Xunit.Abstractions;

namespace Tests.Utils.Tests;

public abstract class TestsWrapper : IAsyncLifetime
{
	public readonly string ExecutePath;

	protected readonly ITestOutputHelper Output;
	public readonly AutoMock Mock;

	public TestsWrapper(ITestOutputHelper output)
	{
		Output = output;

		var type = output.GetType();
		var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
		var test = (ITest)testMember?.GetValue(output)!;

		var testPrefix = test.TestCase.TestMethod.TestClass.Class.Name?.Replace('.', '\\') ?? string.Empty;

		ExecutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestArtifacts", testPrefix,
			test.DisplayName, Guid.NewGuid().ToString("N"));


		var converter = new ConsoleConverter(output, Path.Combine(ExecutePath, "log.txt"));
		Console.SetOut(converter);

		Mock = AutoMock.GetLoose(Configure);
	}
	
	protected virtual void Configure(ContainerBuilder builder) { }

	public virtual Task InitializeAsync()
	{
		return Task.CompletedTask;
	}

	public virtual Task DisposeAsync()
	{
		Mock.Dispose();
		return Task.CompletedTask;
	}

}