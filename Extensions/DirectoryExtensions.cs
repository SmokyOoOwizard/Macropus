namespace Macropus.Extensions;

public static class DirectoryExtensions
{
	public static bool DirectoryExistsAndNotEmpty(string path)
	{
		if (Directory.Exists(path))
			if (DirectoryNotEmpty(path))
				return true;

		return false;
	}

	public static bool DirectoryNotEmpty(string path)
	{
		if (Directory.EnumerateFiles(path).Any()) return true;

		return false;
	}
}