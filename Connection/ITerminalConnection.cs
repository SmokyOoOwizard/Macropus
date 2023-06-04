namespace Connection;

public interface ITerminalConnection : IConnection
{
	int Width { get; }
	int Height { get; }

	ITerminalBuffer<char> ViewBuffer { get; }
	ITerminalBuffer<ConsoleColor> ForegroundBuffer { get; }
	ITerminalBuffer<ConsoleColor> BackgroundBuffer { get; }

	Task ApplyAsync(CancellationToken ctx);
	Task WaitActionsAsync(CancellationToken ctx);
}