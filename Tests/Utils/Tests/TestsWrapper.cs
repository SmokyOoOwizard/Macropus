﻿using Xunit.Abstractions;

namespace Tests.Utils.Tests;

public abstract class TestsWrapper
{
	protected readonly ITestOutputHelper Output;

	public TestsWrapper(ITestOutputHelper output)
	{
		Output = output;
		var converter = new ConsoleConverter(output);
		Console.SetOut(converter);
	}
}