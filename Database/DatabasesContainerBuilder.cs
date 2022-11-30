using Autofac;

namespace Macropus.Database;

public class DatabasesContainerBuilder : Module
{
	protected override void Load(ContainerBuilder builder)
	{
		builder.RegisterType<DatabasesServiceFactory>()
			.AsImplementedInterfaces()
			.SingleInstance();

		builder.RegisterType<DbContextService>()
			.AsImplementedInterfaces()
			.SingleInstance();
	}
}