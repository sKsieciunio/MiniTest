namespace MiniTestRunner;

public class TestResult
{
    private int _numberOfTests;
    private int _testsPassed;
    private string _name;
    
    public int NumberOfTests => _numberOfTests;
    public int TestsPassed => _testsPassed;
    public int TestsFailed => _numberOfTests - _testsPassed;
    

    public TestResult(string name)
    {
        _numberOfTests = 0;
        _testsPassed = 0;
        _name = name;
    }

    public void Add(bool result)
    {
        if (result)
            _testsPassed++;
        _numberOfTests++;
    }

    public void Log(int indent = 0)
    {
        Utils.ConsoleWriteColorLine($"Test summary for: {_name}", ConsoleColor.White, indent);
        Utils.ConsoleWriteColorLine($"* Test passed:  {TestsPassed} / {NumberOfTests}", ConsoleColor.White, indent);    
        Utils.ConsoleWriteColorLine($"* Failed:       {TestsFailed}", ConsoleColor.White, indent);    
    }

    public override string ToString()
    {
        return $"Test passed: {TestsPassed} / {NumberOfTests}, Failed tests: {TestsFailed}";
    }

    public static TestResult operator +(TestResult a, TestResult b)
    {
        var res = new TestResult(a._name);
        res._numberOfTests = a._numberOfTests + b._numberOfTests;
        res._testsPassed = a._testsPassed + b._testsPassed;
        return res;
    }
}