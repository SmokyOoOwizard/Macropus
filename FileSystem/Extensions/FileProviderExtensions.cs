using System.Security.Cryptography;
using Macropus.FileSystem.Interfaces;

namespace Macropus.FileSystem.Extensions;

public static class FileProviderExtensions
{
	public static async Task<string> GetFileHashAsync(this IFileProvider file, CancellationToken cancellationToken)
	{
		using (var md5 = MD5.Create())
		{
			var seek = file.AsStream.Position;

			var hash = await md5.ComputeHashAsync(file.AsStream, cancellationToken);
			var base64String = Convert.ToBase64String(hash);

			file.AsStream.Seek(seek, SeekOrigin.Begin);

			return base64String;
		}
	}
}