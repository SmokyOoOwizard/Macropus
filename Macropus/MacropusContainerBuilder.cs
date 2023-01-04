using Autofac;
using Macropus.Database;
using Macropus.FileSystem;
using Macropus.Project;
using Macropus.Service.Impl;
using Macropus.Web.Api;
using Macropus.Web.Base;
using Macropus.Web.Base.Extensions;
using Macropus.Web.Front;

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
		
		BindWeb(builder);
	}

	private static void BindWeb(ContainerBuilder builder)
	{
		builder.RegisterModule<WebBaseContainerBuilder>();
		builder.RegisterModule<WebFrontContainerBuilder>();
		builder.RegisterWebApiModule<WebApiContainerBuilder>("/api");
	}
}