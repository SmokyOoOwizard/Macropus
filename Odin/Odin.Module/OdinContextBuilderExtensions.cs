using Odin.Context.Builder;

namespace Odin.Module;

public static class OdinContextBuilderExtensions
{
	public static IOdinContextBuilder AddModule(this IOdinContextBuilder contextBuilder, string path)
	{
		return contextBuilder;
	}
}