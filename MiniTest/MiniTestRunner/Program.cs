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

        var assemblies = args.Select(s => AssemblyLoader.LoadAssembly(s)).ToList();
        
    }
}