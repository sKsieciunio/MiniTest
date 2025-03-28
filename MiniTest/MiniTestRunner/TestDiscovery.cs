﻿using System.Reflection;
using MiniTest;

namespace MiniTestRunner;

public class TestDiscovery
{
    public static IEnumerable<Type> GetTestClasses(Assembly assembly)
    {
        return assembly
            .GetTypes().Where(t => t.GetCustomAttributes<TestClassAttribute>().Count() > 0);
    }

    public static MethodInfo? GetBeforeEachMethod(Type testClass)
    {
        return testClass.GetMethods()
            .FirstOrDefault(m => m.GetCustomAttributes<BeforeEachAttribute>().Count() > 0);
    }

    public static MethodInfo? GetAfterEachMethod(Type testClass)
    {
        return testClass.GetMethods()
            .FirstOrDefault(m => m.GetCustomAttributes<AfterEachAttribute>().Count() > 0);
    }

    public static IEnumerable<(MethodInfo, DataRowAttribute[] DataRows)> GetTestMethods(Type testClass)
    {
        return testClass
            .GetMethods()
            .Where(m => m.GetCustomAttributes<TestMethodAttribute>().Count() > 0)
            .Select(m => (m, m.GetCustomAttributes<DataRowAttribute>().ToArray()));
    }
}