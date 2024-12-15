
using System.Reflection;

namespace MiniTestRunner;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: MiniTestRunner <assembly-path1> <assembly-path2> ...");
            return;
        }

        var testRunner = new TestRunner();

        foreach (var arg in args)
        {
            var loadContext = new TestAssemblyLoadContext(arg);

            var assembly = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(arg)));
            testRunner.RunTestsFromAssembly(assembly); 
            
            loadContext.Unload();
        }
    }
}