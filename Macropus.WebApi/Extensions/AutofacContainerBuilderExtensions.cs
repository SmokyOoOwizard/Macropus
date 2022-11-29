using Autofac;
using Autofac.Builder;
using EmbedIO.WebApi;

namespace Macropus.WebApi.Extensions;

public static class AutofacContainerBuilderExtensions
{
	public static void RegisterWebApiModule<T>(this ContainerBuilder builder) where T : AWebApiModule, new()
	{
		builder.Register((ILifetimeScope scope) =>
			{
				var module = new T();
				module.SetLifetimeScope(scope);

				return module;
			})
			.AsSelf()
			.SingleInstance();
	}

	public static IRegistrationBuilder<Func<T>, SimpleActivatorData, SingleRegistrationStyle> RegisterWebApi<T>(
		this ContainerBuilder builder
	) where T : WebApiController
	{
		return builder.Register<Func<T>>(c => c.Resolve<T>);
	}
}