namespace MiniTestRunner;

public static class Logger
{
     public static void LogWarning(string message)
     {
          Console.ForegroundColor = ConsoleColor.Yellow;
          Console.WriteLine(message);
          Console.ResetColor();
     }
     
     public static void LogAssembly(string assemblyName)
     {
          Console.ForegroundColor = ConsoleColor.Blue;
          Console.WriteLine($"Running tests in assembly: {assemblyName}");
          Console.ResetColor();
     }

     public static void LogTestClass(string testClassName)
     {
          Console.ForegroundColor = ConsoleColor.Cyan;
          Console.WriteLine($"Running tests in assembly: {testClassName}");
          Console.ResetColor();
     }

     public static void LogAssemblyResult(TestResult result)
     {
          Console.WriteLine(new string('-', Console.WindowWidth));
          LogResult(result);
          Console.WriteLine(new string('-', Console.WindowWidth));
     }

     public static void LogResult(TestResult result)
     {
          Console.ForegroundColor = ConsoleColor.White;
          result.Log();
          Console.ResetColor();
     }
}