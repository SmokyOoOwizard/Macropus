namespace Odin;




public interface IOdinContext : IDisposable, IAsyncDisposable
{
	/// <summary>
	/// Load raw dependencies
	/// </summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	Task InitAsync(CancellationToken ctx = default);

	/// <summary>
	/// Initialize inner services and run it
	/// </summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	Task StartAsync(CancellationToken ctx = default);

	/// <summary>
	/// Stop inner services
	///
	/// You can just dispose context. stop method need only for reuse already initialized context
	/// </summary>
	/// <param name="ctx"></param>
	/// <returns></returns>
	Task StopAsync(CancellationToken ctx = default);
}