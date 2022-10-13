using Macropus.FileSystem.Impl;
using Tests.Utils;
using Tests.Utils.Tests;
using Xunit.Abstractions;

namespace Tests.FileSystem;

public class FileSystemProviderTests : TestsWithFiles
{
	public FileSystemProviderTests(ITestOutputHelper output) : base(output) { }


	[Fact]
	public async void CreateProviderTest()
	{
		var fsProvider = await FileSystemProvider.Create(ExecutePath).ConfigureAwait(false);

		Assert.NotNull(fsProvider);

		fsProvider.Dispose();
	}
}