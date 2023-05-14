using Odin.Context;

namespace Odin.Tests;

public class ContextCreationsTests
{
	[Fact]
	public async void CreateEmptyContext()
	{
		var contextBuilder = Odin.CreateContextBuilder();
		Assert.NotNull(contextBuilder);

		var context = contextBuilder.Build();
		Assert.NotNull(context);

		await using (context)
		{
			Assert.Equal(EContextStatus.UNKNOWN, context.Status);

			await context.InitAsync();
			Assert.Equal(EContextStatus.Initialize, context.Status);

			await Task.Delay(10);

			await context.StartAsync();
			Assert.Equal(EContextStatus.Run, context.Status);

			await context.StopAsync();
			Assert.Equal(EContextStatus.Stop, context.Status);
		}

		Assert.Equal(EContextStatus.Dispose, context.Status);
	}
}