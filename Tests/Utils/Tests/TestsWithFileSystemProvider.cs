using Macropus.FileSystem;
using Xunit.Abstractions;

#pragma warning disable CS8618

namespace Tests.Utils.Tests;

public abstract class TestsWithFileSystemProvider : TestsWithFiles, IAsyncLifetime
{
	public IFileSystemProvider FileSystemProvider { get; private set; }

	public TestsWithFileSystemProvider(ITestOutputHelper output) : base(output) { }

	public virtual async Task InitializeAsync()
	{
		FileSystemProvider =
			await Macropus.FileSystem.Impl.FileSystemProvider.Create(ExecutePath).ConfigureAwait(false);
	}

	public virtual async Task DisposeAsync()
	{
		await FileSystemProvider.DisposeAsync();
	}
}