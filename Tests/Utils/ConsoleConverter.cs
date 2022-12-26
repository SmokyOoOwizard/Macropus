using System.Text;
using Xunit.Abstractions;

namespace Tests.Utils;

public class ConsoleConverter : TextWriter
{
	private readonly ITestOutputHelper output;

	public ConsoleConverter(ITestOutputHelper output)
	{
		this.output = output;
	}

	public override Encoding Encoding => Encoding.Default;

	public override void WriteLine(string? message)
	{
		output.WriteLine(message);
	}

	public override void WriteLine(string format, params object?[] args)
	{
		output.WriteLine(format, args);
	}

	public override void Write(char value)
	{
		throw new NotSupportedException(
			"This text writer only supports WriteLine(string) and WriteLine(string, params object[]).");
	}
}