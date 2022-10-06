using Autofac;
using Macropus.FileSystem;
using Macropus.FileSystem.Impl;
using Macropus.Module;
using Macropus.Module.Impl;
using Macropus.Project.Instance;

namespace Macropus.Project.Provider.Impl;

internal static class ProjectProviderContainerBuilder
{
    private static IContainer? container;

    public static void Build(ContainerBuilder builder)
    {
        builder.Register((IProjectInformationInternal pi) =>
                FileSystemProvider.Create(pi.Path).ConfigureAwait(false).GetAwaiter().GetResult())
            .As<IFileSystemProvider>().InstancePerLifetimeScope();

        builder.Register((IProjectInformationInternal pi, IFileSystemProvider fs) =>
                ModulesProvider.Create(pi.Path, fs).ConfigureAwait(false).GetAwaiter().GetResult())
            .As<IModulesProvider>().InstancePerLifetimeScope();
    }

    public static IContainer GetContainer()
    {
        if (container != null) return container;

        var builder = new ContainerBuilder();

        Build(builder);
        container = builder.Build();

        return container;
    }
}