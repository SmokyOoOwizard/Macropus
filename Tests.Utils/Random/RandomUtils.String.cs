namespace Tests.Utils.Random;

public static partial class RandomUtils
{
	public static string GetRandomStringOfBase64(int lenght)
	{
		const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
		return GetRandomString(lenght, CHARS);
	}

	public static string GetRandomString(int lenght)
	{
		const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

		var stringChars = new char[lenght];
		var random = System.Random.Shared;

		for (var i = 0; i < stringChars.Length; i++)
			stringChars[i] = CHARS[random.Next(CHARS.Length)];

		return new string(stringChars);
	}

	public static string GetRandomString(int lenght, string availableChars)
	{
		var stringChars = new char[lenght];
		var random = System.Random.Shared;

		for (var i = 0; i < stringChars.Length; i++)
			stringChars[i] = availableChars[random.Next(availableChars.Length)];

		return new string(stringChars);
	}
}