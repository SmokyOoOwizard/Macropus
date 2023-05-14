namespace Odin.Context;

public interface IOdinContext : IDisposable, IAsyncDisposable
{
	EContextStatus Status { get; }

	/// <summary>
	/// Load raw dependencies
	/// </summary>
	Task InitAsync(CancellationToken ctx = default);

	/// <summary>
	/// Initialize inner services and run it
	/// </summary>
	Task StartAsync(CancellationToken ctx = default);

	/// <summary>
	/// Stop inner services
	///
	/// You can just dispose context. stop method need only for reuse already initialized context
	/// </summary>
	Task StopAsync();
}