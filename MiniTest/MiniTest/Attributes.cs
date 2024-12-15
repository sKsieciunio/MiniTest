namespace MiniTest;

[AttributeUsage(AttributeTargets.Class)]
public class TestClassAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class TestMethodAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class BeforeEachAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class AfterEachAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class PriorityAttribute : Attribute
{
    public int Priority { get; }
    public PriorityAttribute(int priority) => Priority = priority;
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class DataRowAttribute : Attribute
{
    public object?[] Parameters { get; } 
    public string Description { get; set; }

    public DataRowAttribute(string description, params object?[] parameters)
    {
        Parameters = parameters;
        Description = description;
    }

    public DataRowAttribute(params object?[] parameters) : this(string.Empty, parameters) { }
}

[AttributeUsage(AttributeTargets.All)]
public class DescriptionAttribute : Attribute
{
    public string Description { get; set; }
    public DescriptionAttribute(string description) => Description = description;
}


