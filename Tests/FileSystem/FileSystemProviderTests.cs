using Macropus.Extensions;
using Tests.Utils;
using Tests.Utils.Random;
using Tests.Utils.Tests;
using Xunit.Abstractions;

namespace Tests.FileSystem;

public class FileSystemProviderTests : TestsWithFileSystemProvider
{
	public FileSystemProviderTests(ITestOutputHelper output) : base(output) { }

	[Fact]
	public async void CreateFileTest()
	{
		var fileName = RandomUtils.GetRandomString(64);

		var fileId = await FileSystemProvider.CreateFileAsync(fileName).ConfigureAwait(false);

		Assert.NotEqual(Guid.Empty, fileId);

		var fileAccess = FileAccess.ReadWrite;
		var fileShare = FileShare.ReadWrite;
		await using var file = await FileSystemProvider.GetFileAsync(fileId, fileAccess, fileShare)
			.ConfigureAwait(false);

		Assert.NotNull(file);

		Assert.Equal(fileName, file.Name);
		Assert.Equal(fileId, file.Id);
		Assert.Equal(fileAccess, file.Access);
		Assert.Equal(fileShare, file.Share);

		Assert.NotNull(file.AsStream);
	}

	[Fact]
	public async void DeleteFileTest()
	{
		var fileName = RandomUtils.GetRandomString(64);

		var fileId = await FileSystemProvider.CreateFileAsync(fileName).ConfigureAwait(false);

		Assert.NotEqual(Guid.Empty, fileId);

		await FileSystemProvider.DeleteFileAsync(fileId).ConfigureAwait(false);

		var file = await FileSystemProvider
			.GetFileAsync(fileId, FileAccess.ReadWrite, FileShare.ReadWrite)
			.WrapException()
			.ConfigureAwait(false);

		if (file.IsFirst)
			Assert.Fail("File must not exist after delete");
	}

	[Fact]
	public async void ReadWriteFileTest()
	{
		var fileName = RandomUtils.GetRandomString(64);

		var fileId = await FileSystemProvider.CreateFileAsync(fileName).ConfigureAwait(false);

		Assert.NotEqual(Guid.Empty, fileId);

		await using var memory = new MemoryStream(RandomUtils.GetRandomByteArray(300));
		{
			await using var file = await FileSystemProvider
				.GetFileAsync(fileId, FileAccess.ReadWrite, FileShare.ReadWrite)
				.ConfigureAwait(false);


			memory.WriteTo(file.AsStream);
			memory.Seek(0, SeekOrigin.Begin);
		}

		{
			await using var file = await FileSystemProvider
				.GetFileAsync(fileId, FileAccess.ReadWrite, FileShare.ReadWrite)
				.ConfigureAwait(false);

			Assert.True(await CompareStreams.StreamsEquals(memory, file.AsStream));
		}
	}
}