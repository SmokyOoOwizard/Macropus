using Delogger.Scope;
using Delogger.Scope.Log;
using Delogger.Scope.Perf;

namespace Tests.Utils.Mock.Delogger;

public class DScope : IDScope
{
	public IDLogger CreateLogger()
	{
		return new DLogger();
	}

	public IDLogger CreateLogger(LoggerCreateOptions options)
	{
		return new DLogger();
	}

	public IDPerfMonitor CreatePerfMonitor()
	{
		return new DPerfMonitor();
	}

	public IDPerfMonitor CreatePerfMonitor(PerfMonitorCreateOptions options)
	{
		return new DPerfMonitor();
	}

	public void Dispose() { }
}