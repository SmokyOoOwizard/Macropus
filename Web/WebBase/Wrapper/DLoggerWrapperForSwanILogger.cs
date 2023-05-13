using Delogger.Scope.Log;
using Swan.Logging;

namespace Macropus.Web.Base.Wrapper;

public class DeloggerWrapperForSwanILogger : ILogger
{
	private readonly IDLogger logger;

	public LogLevel LogLevel { get; } = DebugLogger.IsDebuggerAttached ? LogLevel.Trace : LogLevel.Info;

	public DeloggerWrapperForSwanILogger(IDLogger logger)
	{
		this.logger = logger;
	}


	public void Log(LogMessageReceivedEventArgs logEvent)
	{
		var attachments = new KeyValuePair<string, object>[logEvent.ExtendedData != null ? 2 : 1];
		
		attachments[0] = new("Sequence", logEvent.Sequence);
		if (logEvent.ExtendedData != null)
			attachments[1] = new KeyValuePair<string, object>("ExtendedData", logEvent.ExtendedData);

		logger.Log(
			logEvent.Message,
			new[]
			{
				logEvent.MessageType.ToString()
			},
			null,
			attachments
		);
	}

	public void Dispose()
	{
		logger.Dispose();
	}
}