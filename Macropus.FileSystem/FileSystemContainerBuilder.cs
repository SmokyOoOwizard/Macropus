﻿using Autofac;

namespace Macropus.FileSystem;

public class FileSystemContainerBuilder : Module
{
	protected override void Load(ContainerBuilder builder)
	{
		builder.RegisterType<FileSystemProviderFactory>()
			.AsSelf()
			.AsImplementedInterfaces()
			.SingleInstance();

		builder.RegisterType<FileSystemService>()
			.AsSelf();

		builder.RegisterType<FileProvider>()
			.AsSelf()
			.AsImplementedInterfaces();
	}
}