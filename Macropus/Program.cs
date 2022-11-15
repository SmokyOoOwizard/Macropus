using Autofac;
using Autofac.Extensions.DependencyInjection;
using Delogger.MicrosoftLogging.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Macropus;

internal class Program
{
	private static async Task Main(string[] args)
	{
		var builder = Host.CreateDefaultBuilder(args);

		builder.ConfigureLogging(b => b.ClearProviders());
		builder.SetUpDelogger();
		builder.UseServiceProviderFactory(new AutofacServiceProviderFactory())
			.ConfigureContainer<ContainerBuilder>(b => { b.RegisterModule<MacropusContainerBuilder>(); })
			.UseConsoleLifetime();

		var host = builder.Build();

		await host.StartAsync();
	}
}