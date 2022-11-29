using Autofac;
using Macropus.WebApi.Controllers;
using Macropus.WebApi.Extensions;
using Macropus.WebApi.Service;


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