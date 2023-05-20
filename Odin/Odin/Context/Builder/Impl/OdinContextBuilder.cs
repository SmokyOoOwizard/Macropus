using Autofac;
using Odin.ECS.Impl;

namespace Odin.Context.Builder.Impl;

internal class OdinContextBuilder : IOdinContextBuilder
{
	private readonly ILifetimeScope lifetimeScope;

	public OdinContextBuilder(ILifetimeScope lifetimeScope)
	{
		this.lifetimeScope = lifetimeScope.BeginLifetimeScope(ConfigureBuilder);
	}

	private void ConfigureBuilder(ContainerBuilder builder)
	{
		builder.RegisterType<EcsContexts>()
			.AsImplementedInterfaces();

		builder.RegisterType<OdinContext>()
			.AsImplementedInterfaces();
	}

	public IOdinContext Build()
	{
		return lifetimeScope.Resolve<IOdinContext>();
	}
}