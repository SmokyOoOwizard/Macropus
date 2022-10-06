﻿using System.Reflection;
using Xunit.Abstractions;

namespace Tests.Utils;

public abstract class TestsWithFiles
{
    public readonly string ExecutePath;

    public TestsWithFiles(ITestOutputHelper output)
    {
        var type = output.GetType();
        var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
        var test = (ITest)testMember?.GetValue(output)!;

        var testPrefix = test.TestCase.TestMethod.TestClass.Class.Name?.Replace('.', '\\') ?? string.Empty;

        ExecutePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "TestArtifacts", testPrefix,
            test.DisplayName, Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(ExecutePath);
    }
}