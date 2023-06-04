namespace Tests.Utils.Random;

public static partial class RandomUtils
{
	public static bool GetRandomBool()
	{
		return System.Random.Shared.Next(0, 2) == 1;
	}

	public static byte GetRandomByte(byte from = Byte.MinValue, byte up = Byte.MaxValue)
	{
		return (byte)System.Random.Shared.Next(from, up);
	}

	public static sbyte GetRandomSByte(sbyte from = SByte.MinValue, sbyte up = SByte.MaxValue)
	{
		return (sbyte)System.Random.Shared.Next(from, up);
	}

	public static UInt16 GetRandomUInt16(UInt16 from = UInt16.MinValue, UInt16 up = UInt16.MaxValue)
	{
		return (UInt16)System.Random.Shared.Next(from, up);
	}

	public static Int16 GetRandomInt16(Int16 from = Int16.MinValue, Int16 up = Int16.MaxValue)
	{
		return (Int16)System.Random.Shared.Next(from, up);
	}

	public static uint GetRandomUInt(uint from = UInt32.MinValue, uint up = UInt32.MaxValue)
	{
		return (uint)System.Random.Shared.Next((int)from - int.MaxValue, (int)(up - int.MaxValue - 1));
	}

	public static int GetRandomInt(int from = Int32.MinValue, int up = Int32.MaxValue)
	{
		return System.Random.Shared.Next(from, up);
	}

	public static UInt64 GetRandomUInt64(UInt64 from = UInt64.MinValue, UInt64 up = UInt64.MaxValue)
	{
		return (UInt64)System.Random.Shared.NextInt64((long)from - long.MaxValue, (long)(up - long.MaxValue - 1));
	}

	public static Int64 GetRandomInt64(Int64 from = Int64.MinValue, Int64 up = Int64.MaxValue)
	{
		return System.Random.Shared.NextInt64(from, up);
	}

	public static UInt128 GetRandomUInt128(UInt64 from = UInt64.MinValue, UInt64 up = UInt64.MaxValue)
	{
		return new UInt128(GetRandomUInt64(from, up), GetRandomUInt64(from, up));
	}

	public static Int128 GetRandomInt128(UInt64 from = UInt64.MinValue, UInt64 up = UInt64.MaxValue)
	{
		return new Int128(GetRandomUInt64(from, up), GetRandomUInt64(from, up));
	}

	public static float GetRandomFloat()
	{
		return System.Random.Shared.NextSingle();
	}

	public static double GetRandomDouble()
	{
		return System.Random.Shared.NextSingle();
	}

	public static decimal GetRandomDecimal(Int64 from = Int64.MinValue, Int64 up = Int64.MaxValue)
	{
		return (decimal)System.Random.Shared.NextSingle();
	}
}