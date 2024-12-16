using System.Reflection;
using MiniTest;

namespace MiniTestRunner;

public class TestRunner
{
    public void RunTestsFromAssembly(Assembly assembly)
    {
        Utils.ConsoleWriteColorLine($"Running tests in assembly: {assembly.FullName}\n", ConsoleColor.Blue);
        
        foreach (var testClass in TestDiscovery.GetTestClasses(assembly))
            RunTestClass(testClass);
    }

    private void RunTestClass(Type testClass)
    {
        Utils.ConsoleWriteColorLine($"Running tests in class: {testClass.Name}\n", ConsoleColor.Cyan, 2);

        object? instance;
        try
        {
            instance = Activator.CreateInstance(testClass);
        }
        catch (MissingMethodException)
        {
            Utils.ConsoleWriteColorLine($"No parameterless constructor!\n", ConsoleColor.Yellow, 2); 
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
            .OrderBy(m => m.Item1.GetCustomAttributes<PriorityAttribute>().FirstOrDefault()?.Priority ?? 0)
            .ThenBy(m => m.Item1.Name);
        
        foreach (var (method, parameters) in testMethods)
            foreach (var parameter in parameters)
                RunTest(instance, method, beforeEach, afterEach, parameter);

        Console.WriteLine("");
        

        // foreach (var (method, dataRows) in TestDiscovery.GetParameterizedTests(testClass))
        //     RunTest();
    }

    private void RunTest(object instance, MethodInfo method, Action? beforeEach, Action? afterEach, params object?[] parameters)
    {
        try
        {
            beforeEach?.Invoke();
            method.Invoke(instance, parameters);
            
            Utils.ConsoleWriteColor("[PASSED] ", ConsoleColor.Green, 4);
            Console.WriteLine($"{method.Name}");
        }
        catch (Exception e)
        {
            Utils.ConsoleWriteColor("[FAILED] ", ConsoleColor.Red, 4);
            Console.WriteLine($"{method.Name}");
            
            if (e.InnerException?.Message != null)
                Utils.ConsoleWriteColorLine($"{e.InnerException?.Message}", indent: 6);

            var description = method.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description;
            
            if (description != null)
                Utils.ConsoleWriteColorLine($"{description}", indent: 6);
        }
        finally
        {
            afterEach?.Invoke();
        }
    }
}
