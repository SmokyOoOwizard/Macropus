using Autofac;
using Odin.ECS.Impl;

namespace Odin.Context.Builder;

internal class OdinContextBuilder : IOdinContextBuilder
{
	private readonly ContainerBuilder builder = new();

	public OdinContextBuilder()
	{
		builder.RegisterType<EcsContexts>()
			.AsImplementedInterfaces();

		builder.RegisterType<OdinContext>()
			.AsImplementedInterfaces();
	}

	public IOdinContext Build()
	{
		var container = builder.Build();

		return container.Resolve<IOdinContext>();
	}
}