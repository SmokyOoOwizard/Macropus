using Autofac;
using Macropus.Web.Base;

namespace Macropus.Web.Api;

public class WebApiContainerBuilder : AWebModule
{
	protected override void Configure(ContainerBuilder builder)
	{
		builder.RegisterType<TimeController>()
			.AsSelf()
			.SingleInstance();
	}
}