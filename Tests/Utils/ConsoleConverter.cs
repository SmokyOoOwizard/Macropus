using System.Text;
using Xunit.Abstractions;

namespace Tests.Utils;

public class ConsoleConverter : TextWriter
{
	private readonly ITestOutputHelper output;
	private readonly string logFile;

	private TextWriter? fileWriter;

	public ConsoleConverter(ITestOutputHelper output, string logFile)
	{
		this.output = output;
		this.logFile = logFile;
	}

	public override Encoding Encoding => Encoding.Default;

	public override void WriteLine(string? message)
	{
		if (fileWriter == null)
		{
			fileWriter = new StreamWriter(new FileStream(logFile, FileMode.Append, FileAccess.Write));
		}

		fileWriter.WriteLine(message);
		fileWriter.Flush();
		output.WriteLine(message);
	}

	public override void WriteLine(string format, params object?[] args)
	{
		if (fileWriter == null)
		{
			fileWriter = new StreamWriter(new FileStream(logFile, FileMode.Append, FileAccess.Write));
		}

		fileWriter.WriteLine(format, args);
		fileWriter.Flush();
		output.WriteLine(format, args);
	}

	public override void Write(char value)
	{
		throw new NotSupportedException(
			"This text writer only supports WriteLine(string) and WriteLine(string, params object[]).");
	}
}