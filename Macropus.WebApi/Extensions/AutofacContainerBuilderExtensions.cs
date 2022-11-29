using Autofac;

namespace Macropus.WebApi.Extensions;

public static class AutofacContainerBuilderExtensions
{
	public static void RegisterWebApiModule<T>(this ContainerBuilder builder, string url) where T : AWebApiModule, new()
	{
		builder.Register((ILifetimeScope scope) =>
			{
				var module = new T();
				module.SetLifetimeScope(scope);
				module.Url = url;

				return module;
			})
			.AsSelf()
			.As<AWebApiModule>()
			.SingleInstance();
	}
}