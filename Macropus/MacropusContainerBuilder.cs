using Autofac;
using Macropus.Database;
using Macropus.FileSystem;
using Macropus.Project;
using Macropus.Service.Impl;
using Macropus.WebApi;

namespace Macropus;

public class MacropusContainerBuilder : Module
{
	protected override void Load(ContainerBuilder builder)
	{
		builder.RegisterType<ServiceHost>().InstancePerLifetimeScope();
		builder.RegisterType<MainServiceHost>().AsSelf().AsImplementedInterfaces();

		builder.RegisterType<ConnectionEmulator>().AsSelf().AsImplementedInterfaces();

		builder.RegisterModule<ProjectContainerBuilder>();
		builder.RegisterModule<DatabasesContainerBuilder>();
		builder.RegisterModule<FileSystemContainerBuilder>();
		builder.RegisterModule<WebApiContainerBuilder>();
	}
}