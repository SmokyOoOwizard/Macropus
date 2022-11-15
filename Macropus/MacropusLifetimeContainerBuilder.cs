using Autofac;
using Macropus.Service.Impl;

namespace Macropus;

public class MacropusContainerBuilder : Autofac.Module
{
	protected override void Load(ContainerBuilder builder)
	{
		builder.RegisterType<ServiceHost>().InstancePerLifetimeScope();
		builder.RegisterType<MainServiceHost>().AsSelf().AsImplementedInterfaces();
	}
}