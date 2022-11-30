using Autofac;
using Macropus.Web.Base.Controllers;
using Macropus.Web.Base.Extensions;
using Macropus.Web.Base.Service;

namespace Macropus.Web.Base;

public class WebApiContainerBuilder : Module
{
	protected override void Load(ContainerBuilder builder)
	{
		builder.RegisterType<WebApiService>()
			.AsImplementedInterfaces()
			.SingleInstance();
		
		builder.RegisterWebApiModule<BaseWebModule>("/");
	}
}