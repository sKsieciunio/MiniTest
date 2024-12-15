using System.Reflection;
using System.Runtime.Loader;

namespace MiniTestRunner;

public static class AssemblyLoader
{
    public static Assembly LoadAssembly(string path)
    {
        var loadContext = new AssemblyLoadContext(null, isCollectible: true);
        var assembly = loadContext.LoadFromAssemblyPath(path);
        loadContext.Unload();
        return assembly;
    }
}