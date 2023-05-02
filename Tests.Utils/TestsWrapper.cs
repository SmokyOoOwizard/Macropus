using System.Reflection;
using Autofac;
using Tests.Utils.Mock.Delogger;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Utils;

public abstract class TestsWrapper : IAsyncLifetime
{
	public readonly string ExecutePath;

	protected readonly ITestOutputHelper Output;

	public readonly IContainer Container;

	public TestsWrapper(ITestOutputHelper output)
	{
		Output = output;

		var type = output.GetType();
		var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
		var test = (ITest)testMember?.GetValue(output)!;

		var testPrefix = test.TestCase.TestMethod.TestClass.Class.Name?.Replace('.', '\\') ?? string.Empty;

		ExecutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestArtifacts", testPrefix,
			test.DisplayName, Guid.NewGuid().ToString("N"));


		var converter = new ConsoleConverter(output);
		Console.SetOut(converter);

		var containerBuilder = new ContainerBuilder();

		// ReSharper disable once VirtualMemberCallInConstructor
		Configure(containerBuilder);

		Container = containerBuilder.Build();
	}

	protected virtual void Configure(ContainerBuilder builder)
	{
		builder.RegisterType<DLogger>()
			.AsSelf()
			.AsImplementedInterfaces();

		builder.RegisterType<DScope>()
			.AsSelf()
			.AsImplementedInterfaces();

		builder.RegisterType<DPerfMonitor>()
			.AsSelf()
			.AsImplementedInterfaces();
	}

	public virtual Task InitializeAsync()
	{
		return Task.CompletedTask;
	}

	public virtual async Task DisposeAsync()
	{
		await Container.DisposeAsync();
	}
}