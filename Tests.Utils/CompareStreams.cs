// ReSharper disable InconsistentNaming

namespace Tests.Utils;

public static class CompareStreams
{
	public static async Task<bool> StreamsEquals(Stream s1, Stream s2, CancellationToken cancellationToken = default)
	{
		const int bufferSize = 1024 * (1 << 16);

		var s1_Buffer = new byte[bufferSize];
		var s2_Buffer = new byte[bufferSize];

		int s1_Read;
		do
		{
			s1_Read = await s1.ReadAsync(s1_Buffer, cancellationToken).ConfigureAwait(false);
			var s2_Read = await s2.ReadAsync(s2_Buffer, cancellationToken).ConfigureAwait(false);

			if (s1_Read != s2_Read)
				return false;

			if (!s1_Buffer.SequenceEqual(s2_Buffer))
				return false;
		} while (s1_Read == 0);

		return true;
	}
}