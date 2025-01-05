namespace IRGenerator.Builder;

public class IRFunction
{
    public string Name { get; set; }
    public List<IRInstruction> Instructions { get; set; } = new(); // Linear IR, top to bottom
    public List<IRVariable> Parameters { get; set; } = new();
    public List<IRVariable> Locals { get; set; } = new();
    public IRType ReturnType { get; set; }
    
    public static IRFunction Create(string name, IRType returnType, params IRVariable[] parameters)
    {
        return new IRFunction
        {
            Name = name,
            ReturnType = returnType,
            Parameters = parameters.ToList()
        };
    }
}