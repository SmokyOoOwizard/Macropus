using Autofac;
using EmbedIO;
using EmbedIO.Files;

namespace Macropus.Web.Front;

public class WebFrontContainerBuilder : Module
{
	protected override void Load(ContainerBuilder builder)
	{
		builder.RegisterInstance(new FileModule("/",
			new ResourceFileProvider(typeof(WebFrontContainerBuilder).Assembly, @"Macropus.Web.Front.Resources.Front")))
			.As<IWebModule>();
	}
}