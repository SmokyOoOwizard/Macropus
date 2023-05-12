using Delogger;
using Delogger.Scope.Perf;

namespace Tests.Utils.Mock.Delogger;

public class DPerfMonitor : IDPerfMonitor
{
	private Dictionary<string, object> attachments = new();
	private DLogger dLogger = new();

	public void Log(string message, string[]? tags = null, object[]? args = null, KeyValuePair<string, object>[]? attachments = null, WriteFlagsEnum flags = WriteFlagsEnum.All)
	{
		var a = new List<KeyValuePair<string, object>>(this.attachments);
		if (attachments != null)
			a.AddRange(attachments);

		dLogger.Log(message, tags, args, a.ToArray(), flags);
	}

	public void AddAttachment(string key, object attachment)
	{
		attachments[key] = attachment;
	}

	public void Dispose() { }
}