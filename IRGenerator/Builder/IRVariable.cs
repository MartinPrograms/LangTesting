namespace IRGenerator.Builder;

public class IRVariable
{
    public string Name { get; set; }
    public IRType Type { get; set; }
    public string Value { get; set; }
    public bool Constant { get; set; }
    
    public IRVariable(string name, IRType type, string value, bool const2 = false)
    {
        Name = name;
        Type = type;
        Value = value;
        Constant = const2;
    }
    
    public override string ToString()
    {
        return $"{Name} {Type} {Value}";
    }
}

public enum IRType
{
    Int,
    Float,
    String,
    Bool,
    Pointer,
    Void,
    Unknown
}

public static class IRTypeHelper
{
    public static string GetIRType(IRType type)
    {
        return type switch
        {
            IRType.Int => "int",
            IRType.Float => "float",
            IRType.String => "string",
            IRType.Bool => "bool",
            IRType.Pointer => "pointer",
            IRType.Void => "void",
            _ => "unknown"
        };
    }
    
    public static IRType GetIRType(string type)
    {
        return type switch
        {
            "int" => IRType.Int,
            "float" => IRType.Float,
            "string" => IRType.String,
            "bool" => IRType.Bool,
            "pointer" => IRType.Pointer,
            "void" => IRType.Void,
            _ => IRType.Unknown
        };
    }
}