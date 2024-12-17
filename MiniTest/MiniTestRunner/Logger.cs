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
          LogResults(result);
          Console.WriteLine(new string('-', Console.WindowWidth));
     }

     public static void LogResults(TestResult results)
     {
          Console.ForegroundColor = ConsoleColor.White;
          results.Log();
          Console.ResetColor();
     }

     public static void LogTest(bool result, string message)
     {
          if (result)
          {
               Console.ForegroundColor = ConsoleColor.Green;
               Console.Write($"[PASSED] ");
          }
          else
          {
               Console.ForegroundColor = ConsoleColor.Red;
               Console.Write($"[FAILED] ");
          }

          Console.ResetColor();
          Console.WriteLine(message);
     }
}