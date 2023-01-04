using Autofac;
using Macropus.Web.Base.Service;

namespace Macropus.Web.Base;

public class WebBaseContainerBuilder : Module
{
	protected override void Load(ContainerBuilder builder)
	{
		builder.RegisterType<WebService>()
			.AsImplementedInterfaces()
			.SingleInstance();
	}
}