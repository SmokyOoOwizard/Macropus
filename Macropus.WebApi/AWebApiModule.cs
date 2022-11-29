using Autofac;
using EmbedIO.WebApi;

namespace Macropus.WebApi;

public abstract class AWebApiModule
{
	private ILifetimeScope? moduleScope;

	private ILifetimeScope? configuredScope;

	public void SetLifetimeScope(ILifetimeScope scope)
	{
		moduleScope = scope;

		configuredScope?.Dispose();
		configuredScope = moduleScope.BeginLifetimeScope(Configure);
	}


	protected abstract void Configure(ContainerBuilder builder);

	public T ResolveController<T>() where T : WebApiController
	{
		if (configuredScope == null)
			// TODO
			throw new Exception();

		return configuredScope.Resolve<T>();
	}

	public IEnumerable<Type> GetWebApiControllers()
	{
		if (configuredScope == null)
			return Array.Empty<Type>();

		return configuredScope.ComponentRegistry.Registrations
			.Where(r => typeof(WebApiController).IsAssignableFrom(r.Activator.LimitType))
			.Select(r => r.Activator.LimitType);
	}
}