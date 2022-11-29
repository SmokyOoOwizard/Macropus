using Autofac;

namespace Macropus.WebApi;

public class TestApiModule : AWebApiModule
{
	protected override void Configure(ContainerBuilder builder)
	{
		builder.RegisterType<PeopleController>()
			.SingleInstance();
	}
}