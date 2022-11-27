using Autofac;


namespace Macropus.WebApi;

public class WebApiContainerBuilder : Module
{
	protected override void Load(ContainerBuilder builder)
	{
		builder.RegisterType<WebApiService>()
			.AsImplementedInterfaces()
			.SingleInstance();
	}
}