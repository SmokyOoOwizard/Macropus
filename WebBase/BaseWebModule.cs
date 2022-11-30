using Autofac;
using Macropus.Web.Base.Controllers;

namespace Macropus.Web.Base;

public class BaseWebModule : AWebModule
{
	protected override void Configure(ContainerBuilder builder)
	{
		builder.RegisterType<NotFoundController>()
			.SingleInstance();
	}
}