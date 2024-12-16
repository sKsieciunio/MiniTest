using System.Reflection;

namespace MiniTest;

public class AssertionException : Exception
{
    public AssertionException(string message) : base(message) { }
}

public static class Assert
{
    public static void Fail(string message = "")
    {
        throw new AssertionException($"Fail. {message}");
    }
    
    public static void IsTrue(bool condition, string message = "")
    {
        if (!condition)
            throw new AssertionException($"Condition is false. {message}");
    }
    
    public static void IsFalse(bool condition, string message = "")
    {
        if (condition)
            throw new AssertionException($"Condition is true. {message}");
    }

    public static void AreEqual<T>(T? expected, T? actual, string message = "")
    {
        if (!Equals(expected, actual))
            throw new AssertionException($"Expected: {expected?.ToString() ?? "null"}. Actual: {actual?.ToString() ?? "null"}. {message}");
    }

    public static void AreNotEqual<T>(T? notExpected, T? actual, string message = "")
    {
        if (Equals(notExpected, actual))
            throw new AssertionException($"Did not expect: {notExpected?.ToString() ?? "null"}. Actual: {actual?.ToString() ?? "null"}. {message}");
    }

    public static void ThrowsException<TException>(Action action, string message = "")
    {
        try
        {
            action();
            throw new AssertionException(
                $"Expected expception type: <{typeof(TException)}> but no exception was thrown. {message}");
        }
        catch (Exception e) when (!(e is TException))
        {
            throw new AssertionException(
                $"Expected exception type: <{typeof(TException)}>. Actual exception type: <{e.GetType()}>. {message}");
        }
        catch (Exception e) when (e is TException) { }
    }
}