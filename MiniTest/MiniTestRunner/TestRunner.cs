﻿using System.Reflection;
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
            Logger.LogResult(results);
            return results;
        }

        if (instance == null)
        {
            Logger.LogWarning("Failed to instanciate class!");
            Logger.LogResult(results);
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
                results.Add(RunTest(instance, method, beforeEach, afterEach));

            foreach (var parameter in parameters)
                results.Add(RunTest(instance, method, beforeEach, afterEach, parameter.Parameters));
        }

        Logger.LogResult(results);
        
        return results;
    }

    private bool RunTest(object instance, MethodInfo method, Action? beforeEach, Action? afterEach, params object[] parameters)
    {
        bool result;
        
        try
        {
            beforeEach?.Invoke();
            method.Invoke(instance, parameters);
            
            Utils.ConsoleWriteColor("[PASSED] ", ConsoleColor.Green, 4);
            Console.WriteLine(parameters.Length == 0 ? $"{method.Name}" : $"{method.Name} -> [Description Here]");

            result = true;
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

            result = false;
        }
        finally
        {
            afterEach?.Invoke();
        }

        return result;
    }
}
