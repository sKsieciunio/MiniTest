﻿using System.Reflection;
using System.Runtime.Loader;

namespace MiniTestRunner;

public class TestAssemblyLoadContext : AssemblyLoadContext
{
    private AssemblyDependencyResolver _resolver;

    public TestAssemblyLoadContext(string path) : base(isCollectible: true)
    {
        _resolver = new AssemblyDependencyResolver(path);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        Assembly? assemblyInDefaultContext = Default.Assemblies.FirstOrDefault(assembly => assembly.FullName == assemblyName.FullName);

        if (assemblyInDefaultContext is not null)
        {
            return assemblyInDefaultContext;
        }

        string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        if(assemblyPath != null)
        {
            return LoadFromAssemblyPath(assemblyPath);
        }
        return null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if(libraryPath != null)
        {
            return LoadUnmanagedDllFromPath(libraryPath);
        }

        return IntPtr.Zero;
    }
}
