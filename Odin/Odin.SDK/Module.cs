using Autofac;

namespace Odin.SDK;

public abstract class Module
{
	public abstract void Configure(ContainerBuilder builder);

	public virtual void ConfigureECS(IECSBuilder builder) { }
}