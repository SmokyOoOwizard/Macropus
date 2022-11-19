using Autofac;
using Macropus.FileSystem;
using Macropus.FileSystem.Impl;
using Macropus.Module;
using Macropus.Module.Impl;
using Macropus.Project;
using Macropus.Project.Provider.Impl;
using Macropus.Project.Storage.Impl;

namespace Macropus;

public class ProjectContainerBuilder : Autofac.Module
{
	protected override void Load(ContainerBuilder builder)
	{
		builder.RegisterType<ProjectsStorageMaster>()
			.AsSelf()
			.AsImplementedInterfaces()
			.SingleInstance();

		builder.RegisterType<ProjectsStorageLocalFactory>()
			.AsSelf()
			.SingleInstance();

		builder.RegisterType<ProjectProviderFactory>()
			.AsSelf()
			.SingleInstance();

		builder.Register((IProjectInformationInternal pi) =>
				FileSystemProvider.Create(pi.Path).ConfigureAwait(false).GetAwaiter().GetResult())
			.As<IFileSystemProvider>()
			.InstancePerLifetimeScope();

		builder.Register((IProjectInformationInternal pi, IFileSystemProvider fs) =>
				ModulesProvider.Create(pi.Path, fs).ConfigureAwait(false).GetAwaiter().GetResult())
			.As<IModulesProvider>()
			.InstancePerLifetimeScope();
	}
}