using Autofac;
using Macropus.Project.Connection.Impl;
using Macropus.Project.Instance.Impl;
using Macropus.Project.Raw.Impl;
using Macropus.Project.Storage.Impl;
using Macropus.Project.Storage.Raw.Impl;

namespace Macropus.Project;

public class ProjectContainerBuilder : Module
{
	protected override void Load(ContainerBuilder builder)
	{
		RegisterStorage(builder);
		RegisterProjectInstance(builder);
		RegisterRawProject(builder);
		RegisterProjectConnection(builder);
	}

	private void RegisterProjectInstance(ContainerBuilder builder)
	{
		builder.RegisterType<ProjectService>()
			.AsSelf()
			.AsImplementedInterfaces()
			.SingleInstance();

		builder.RegisterType<ProjectInstance>()
			.AsSelf()
			.AsImplementedInterfaces();
	}

	private void RegisterRawProject(ContainerBuilder builder)
	{
		builder.RegisterType<RawProjectService>()
			.AsSelf()
			.AsImplementedInterfaces()
			.SingleInstance();

		builder.RegisterType<RawProjectFactory>()
			.AsSelf()
			.AsImplementedInterfaces()
			.SingleInstance();
	}


	private void RegisterProjectConnection(ContainerBuilder builder)
	{
		builder.RegisterType<ProjectConnection>()
			.AsSelf()
			.AsImplementedInterfaces();

		builder.RegisterType<ConnectionService>()
			.AsSelf()
			.AsImplementedInterfaces()
			.SingleInstance();
	}

	private void RegisterStorage(ContainerBuilder builder)
	{
		builder.RegisterType<ProjectsStorageMaster>()
			.AsSelf()
			.AsImplementedInterfaces()
			.SingleInstance();

		builder.RegisterType<ProjectsStorageLocalFactory>()
			.AsSelf()
			.SingleInstance();

		builder.RegisterType<ProjectsStorageLocal>()
			.AsSelf();
	}
}