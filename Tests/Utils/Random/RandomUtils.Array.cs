// ReSharper disable StringLiteralTypo

namespace Tests.Utils.Random;

public static partial class RandomUtils
{
	public static byte[] GetRandomByteArray(int lenght)
	{
		var buffer = new byte[lenght];
		var random = new System.Random();

		for (var i = 0; i < buffer.Length; i++)
			buffer[i] = (byte)random.Next(256);

		return buffer;
	}
}