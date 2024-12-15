using System.Reflection;
using MiniTest;

namespace MiniTestRunner;

public class TestRunner
{
    public void RunTestsFromAssembly(Assembly assembly)
    {
        ConsoleWriteColorLine($"Running tests in assembly: {assembly.FullName}\n", ConsoleColor.Blue);
        
        foreach (var testClass in TestDiscovery.GetTestClasses(assembly))
            RunTestClass(testClass);
    }

    private void RunTestClass(Type testClass)
    {
        ConsoleWriteColorLine($"Running tests in class: {testClass.Name}\n", ConsoleColor.Cyan, 2);

        object? instance;
        try
        {
            instance = Activator.CreateInstance(testClass);
        }
        catch (MissingMethodException)
        {
            ConsoleWriteColorLine($"No parameterless constructor!\n", ConsoleColor.Yellow, 2); 
            return;
        }

        if (instance == null)
            return;

        var beforeEachMethodInfo = TestDiscovery.GetBeforeEachMethod(testClass);
        var afterEachMethodInfo = TestDiscovery.GetAfterEachMethod(testClass);

        Action? beforeEach = beforeEachMethodInfo != null 
            ? (Action)Delegate.CreateDelegate(typeof(Action), instance, beforeEachMethodInfo)
            : null;
        
        Action? afterEach = afterEachMethodInfo != null 
            ? (Action)Delegate.CreateDelegate(typeof(Action), instance, afterEachMethodInfo)
            : null;

        var testMethods = TestDiscovery.GetTestMethods(testClass)
            .OrderBy(m => m.GetCustomAttributes<PriorityAttribute>().FirstOrDefault()?.Priority ?? 0)
            .ThenBy(m => m.Name);
        
        RunTests(instance, testMethods, beforeEach, afterEach);

        // foreach (var (method, dataRows) in TestDiscovery.GetParameterizedTests(testClass))
        //     RunTest();
    }

    private void RunTests(object instance, IEnumerable<MethodInfo> methods, Action? beforeEach, Action? afterEach, params object?[] parameters)
    {
        foreach (var method in methods)
        {
            try
            {
                beforeEach?.Invoke();
                method.Invoke(instance, parameters);
                ConsoleWriteColorLine($"[PASSED] {method.Name} ", ConsoleColor.Green, 4);
            }
            catch (Exception e)
            {
                ConsoleWriteColorLine($"[FAILED] {method.Name} ", ConsoleColor.Red, 4);
                
                if (e.InnerException?.Message != null)
                    ConsoleWriteColorLine($"{e.InnerException?.Message}", indent: 6);

                var description = method.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description;
                
                if (description != null)
                    ConsoleWriteColorLine($"{description}", indent: 6);
            }
            finally
            {
                afterEach?.Invoke();
            }
        }

        Console.WriteLine("");
    }

    private void ConsoleWriteColorLine(string message, ConsoleColor color = ConsoleColor.Gray, int indent = 0)
    {
        var indentStr = new string(' ', indent);
        Console.ForegroundColor = color;
        Console.WriteLine($"{indentStr}{message}");
        Console.ResetColor();
    }
}
