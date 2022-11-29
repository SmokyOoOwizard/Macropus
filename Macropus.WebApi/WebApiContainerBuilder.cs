using Autofac;
using Macropus.WebApi.Extensions;


namespace Macropus.WebApi;

public class WebApiContainerBuilder : Module
{
	protected override void Load(ContainerBuilder builder)
	{
		builder.RegisterType<WebApiService>()
			.AsImplementedInterfaces()
			.SingleInstance();
		
		builder.RegisterWebApiModule<TestApiModule>("/api");
	}
}