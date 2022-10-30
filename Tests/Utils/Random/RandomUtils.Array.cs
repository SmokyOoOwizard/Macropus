// ReSharper disable StringLiteralTypo

namespace Tests.Utils.Random;

public static partial class RandomUtils
{
	public static byte[] GetRandomByteArray(int lenght)
	{
		var buffer = new byte[lenght];
		var random = System.Random.Shared;

		for (var i = 0; i < buffer.Length; i++)
			buffer[i] = (byte)random.Next(256);

		return buffer;
	}

	public static float[] GetRandomFloatArray(int lenght)
	{
		var buffer = new float[lenght];
		var random = System.Random.Shared;

		for (var i = 0; i < buffer.Length; i++)
			buffer[i] = random.NextSingle();

		return buffer;
	}
}