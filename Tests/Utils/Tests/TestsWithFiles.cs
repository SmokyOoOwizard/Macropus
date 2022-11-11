using System.Reflection;
using Xunit.Abstractions;

namespace Tests.Utils.Tests;

public abstract class TestsWithFiles : TestsWrapper
{
	public TestsWithFiles(ITestOutputHelper output) : base(output)
	{
		Directory.CreateDirectory(ExecutePath);
	}
}