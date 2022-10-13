namespace Tests.Utils.Random;

public static partial class RandomUtils
{
	public static uint GetRandomUInt(uint from, uint up)
	{
		return (uint)new System.Random().Next((int)from, (int)up);
	}
}