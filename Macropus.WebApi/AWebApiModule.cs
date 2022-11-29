using Autofac;
using EmbedIO.Routing;
using EmbedIO.WebApi;

namespace Macropus.WebApi;

public abstract class AWebApiModule
{
	public string Url { get; internal set; }

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

	public IEnumerable<KeyValuePair<Type, RouteAttribute>> GetEndpoints()
	{
		foreach (var controller in GetWebApiControllers())
		{
			var methods = controller.GetMethods();
			foreach (var method in methods)
			{
				var attributes = method.GetCustomAttributes(typeof(RouteAttribute), true);
				if (attributes.Length == 0)
					continue;

				yield return new KeyValuePair<Type, RouteAttribute>(controller, (RouteAttribute)attributes[0]);
			}
		}
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