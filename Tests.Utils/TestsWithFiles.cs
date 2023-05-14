using Xunit.Abstractions;

namespace Tests.Utils;

public abstract class TestsWithFiles : TestsWrapper
{
	public TestsWithFiles(ITestOutputHelper output) : base(output)
	{
		Directory.CreateDirectory(ExecutePath);
	}
}