using Odin.Context.Builder;

namespace Odin;

public static class Odin
{
	public static IOdinContextBuilder CreateContextBuilder()
	{
		return new OdinContextBuilder();
	}
}