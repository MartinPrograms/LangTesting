using System.Text;

namespace IRGenerator.Builder;

public class IRBuilder
{
    public string ModuleName { get; set; }
    
    public List<IRVariable> Globals { get; set; } = new();
    public List<IRFunction> Functions { get; set; } = new();
    
    public IRBuilder(string moduleName)
    {
        ModuleName = moduleName;
    }
    #region AddGlobal

    void CheckGlobalName(string name)
    {
        if (Globals.Any(g => g.Name == name))
        {
            throw new Exception($"Global with name {name} already exists");
        }
    }

    public void AddGlobal(IRVariable global)
    {
        CheckGlobalName(global.Name);
        Globals.Add(global);
    }
    
    public void AddGlobal(string name, IRType type)
    {
        CheckGlobalName(name);
        Globals.Add(new IRVariable(name, type, ""));
    }
    
    public void AddGlobal(string name, IRType type, string value)
    {
        CheckGlobalName(name);
        Globals.Add(new IRVariable(name, type, value));
    }
    
    public void AddGlobal(string name, IRType type, string value, bool isConstant)
    {
        CheckGlobalName(name);
        Globals.Add(new IRVariable(name, type, value, isConstant));
    }
    
    public IRVariable GetGlobal(string name)
    {
        if (!Globals.Any(g => g.Name == name))
        {
            throw new Exception($"Global with name {name} does not exist");
        }
        
        return Globals.FirstOrDefault(g => g.Name == name);
    }
    
    #endregion

    #region AddFunction
    
    public void AddFunction(IRFunction function)
    {
        if (Functions.Any(f => f.Name == function.Name))
        {
            throw new Exception($"Function with name {function.Name} already exists");
        }
        
        Functions.Add(function);
    }
    
    public void AddFunction(string name, IRType returnType, List<IRVariable> parameters)
    {
        if (Functions.Any(f => f.Name == name))
        {
            throw new Exception($"Function with name {name} already exists");
        }
        
        Functions.Add(IRFunction.Create(name, returnType, parameters.ToArray()));
    }

    public IRFunction GetFunction(string name)
    {
        if (!Functions.Any(f => f.Name == name))
        {
            throw new Exception($"Function with name {name} does not exist");
        }
        
        return Functions.FirstOrDefault(f => f.Name == name);
    }
    
    #endregion
    
    public string Build()
    {
        var builder = new StringBuilder();
        
        // It is NOT an LLVM IR builder, it's a very simple ir builder, which can only be compiled by a custom compiler
        builder.AppendLine($"beginmodule {ModuleName}");
        
        foreach (var global in Globals)
        {
            builder.AppendLine($"global {global.Type} {global.Name} = {global.Value}");
        }
        
        foreach (var function in Functions)
        {
            builder.AppendLine($"function {function.ReturnType} {function.Name}({string.Join(", ", function.Parameters)})");
        }
        
        builder.AppendLine("endmodule");
        
        builder.AppendLine("beginfunctions");
        
        foreach (var function in Functions)
        {
            builder.AppendLine($"beginfunction {function.Name}");

            builder.AppendLine("beginlocals");
            foreach (var local in function.Locals)
            {
                builder.AppendLine($"local {local.Type} {local.Name} = {local.Value}");
            }
            
            builder.AppendLine("endlocals");
            
            builder.AppendLine("begininstructions");
            
            foreach (var block in function.Instructions)
            {
                builder.AppendLine(block.ToString());
            }
            
            builder.AppendLine("endinstructions");
            
            builder.AppendLine("endfunction");
        }
        
        builder.AppendLine("endfunctions");
        
        return builder.ToString();
    }
}