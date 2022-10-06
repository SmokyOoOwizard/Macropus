using System.Reflection;
using Macropus.Interfaces.Module;

namespace Macropus.Module.Extensions;

internal static class AssemblyExtensions
{
    public static Type GetModuleEntryPoint(this Assembly assembly)
    {
        var posiblyModulEntryPoints = assembly.GetTypes().Where(t => t.IsDefined(typeof(ModuleAttribute), true));
        if (!posiblyModulEntryPoints.Any())
            // TODO: throw module entry point not find
            throw new Exception();

        var notAbstractEntryPoints = posiblyModulEntryPoints.Where(t => !t.IsAbstract);
        if (!notAbstractEntryPoints.Any())
            // TODO: throw module not contains not abstract entry point
            throw new Exception();

        var ctrParameterless = notAbstractEntryPoints.Where(t => t.GetConstructor(Type.EmptyTypes) != null);
        if (!ctrParameterless.Any())
            // TODO: throw module not contains parameterless entry point
            throw new Exception();
        if (ctrParameterless.Count() > 1)
            // TODO: throw module contains more 1 entry point
            throw new Exception();

        return ctrParameterless.First();
    }
}