using AnyOfTypes;

namespace Macropus.Extensions;

public static class TaskExtensions
{
	public static async Task<AnyOf<T, Exception>> WrapException<T>(this Task<T> task)
	{
		try
		{
			return await task;
		}
		catch (Exception ex)
		{
			return ex;
		}
	}

	public static Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
	{
		return task.IsCompleted // fast-path optimization
			? task
			: task.ContinueWith(
				completedTask => completedTask.GetAwaiter().GetResult(),
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
	}
}