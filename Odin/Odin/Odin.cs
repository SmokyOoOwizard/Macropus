using Autofac;
using Odin.Context.Builder;
using Odin.Context.Builder.Impl;

namespace Odin;

public static class Odin
{
	public static IOdinContextBuilder CreateContextBuilder(ILifetimeScope scope)
	{
		return new OdinContextBuilder(scope);
	}
}