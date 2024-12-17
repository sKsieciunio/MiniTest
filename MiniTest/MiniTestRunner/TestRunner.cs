using System.Reflection;
using MiniTest;

namespace MiniTestRunner;

public class TestRunner
{
    public void RunTestsFromAssembly(Assembly assembly)
    {
        Logger.LogAssembly(assembly.FullName ?? "");

        var results = new TestResult(assembly.FullName ?? "null");
        
        foreach (var testClass in TestDiscovery.GetTestClasses(assembly))
            results += RunTestClass(testClass);

        Logger.LogAssemblyResult(results);
    }

    private TestResult RunTestClass(Type testClass)
    {
        Logger.LogTestClass(testClass.Name);

        var results = new TestResult(testClass.Name);

        object? instance;
        try
        {
            instance = Activator.CreateInstance(testClass);
        }
        catch (MissingMethodException)
        {
            Logger.LogWarning("No parameterless constructor!");
            Logger.LogResults(results);
            return results;
        }

        if (instance == null)
        {
            Logger.LogWarning("Failed to instanciate class!");
            Logger.LogResults(results);
            return results;
        }

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
        {
            if (parameters.Length == 0)
                results.Add(RunTest(instance, method, beforeEach, afterEach, []));
            else
            {
                Console.WriteLine($"[ TEST ] {method.Name}");
                
                foreach (var parameter in parameters)
                    results.Add(RunTest(instance, method, beforeEach, afterEach, parameter.Parameters, parameter.Description));
            }
            
            var description = method.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description;
            
            if (description != null)
                Console.WriteLine($"{description}");
        }

        Logger.LogResults(results);
        
        return results;
    }

    private bool RunTest(object instance, MethodInfo method, Action? beforeEach, Action? afterEach, object[] parameters, string parameterDescription = "")
    {
        bool result = false;
        string indent = parameters.Length == 0 ? "" : "-> ";
        Console.Write(indent);
        
        try
        {
            beforeEach?.Invoke();
            method.Invoke(instance, parameters);
            
            result = true;
        }
        catch (Exception e)
        {
            result = false;
            
            if (e.InnerException?.Message != null)
                Console.WriteLine($"{e.InnerException?.Message}");
        }
        finally
        {
            Logger.LogTest(result, parameters.Length == 0 ? $"{method.Name}" : $"{parameterDescription}");
            
            afterEach?.Invoke();
        }

        return result;
    }
}
