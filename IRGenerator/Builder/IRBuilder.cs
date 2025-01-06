using System.Text;

namespace IRGenerator.Builder;

public class IRBuilder
{
    public string ModuleName { get; set; }
    public IRPlatform Platform { get; set; }
    public bool HasMainFunction => Functions.Any(f => f.Name == "main");
    public List<IRVariable> Globals { get; set; } = new();
    public List<IRFunction> Functions { get; set; } = new();
    
    public IRBuilder(string moduleName, IRPlatform platform)
    {
        ModuleName = moduleName;
        Platform = platform;
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
        builder.AppendLine("%platform " + Platform);
        builder.AppendLine("");
        builder.AppendLine($"%beginmodule {ModuleName}");
        
        foreach (var global in Globals)
        {
            builder.AppendLine($"\tglobal {global.Type} {global.Name} = {global.Value}");
        }
        
        foreach (var function in Functions)
        {
            builder.AppendLine($"\tfunction {function.ReturnType} {function.Name}({string.Join(", ", function.Parameters)})");
        }
        
        builder.AppendLine("%endmodule");
        builder.AppendLine("");
        
        builder.AppendLine("%beginfunctions");
        
        foreach (var function in Functions)
        {
            builder.AppendLine($"\t%beginfunction {function.Name}");

            builder.AppendLine("\t%beginlocals");
            foreach (var local in function.Locals)
            {
                builder.AppendLine($"\t\tlocal {local.Type} {local.Name}");
            }
            
            builder.AppendLine("\t%endlocals");
            
            builder.AppendLine("\t%begininstructions");
            
            foreach (var block in function.Instructions)
            {
                if (block.OpCode == IROpCode.Label)
                {
                    builder.AppendLine(""+block.ToString() + ":"); // making labels stand out
                }else
                    builder.AppendLine("\t\t"+block.ToString());
            }
            
            builder.AppendLine("\t%endinstructions");
            
            builder.AppendLine("\t%endfunction");
            
            if (function != Functions.Last())
            {
                builder.AppendLine("");
            }
        }
        
        builder.AppendLine("%endfunctions");
        
        return builder.ToString();
    }

    public bool HasGlobal(string variableValue)
    {
        return Globals.Any(g => g.Name == variableValue);
    }

    public bool HasFunction(string variableValue)
    {
        return Functions.Any(f => f.Name == variableValue);
    }

    private int _variableCounter = 0;
    public string GetNextVariableName()
    {
        return $"_v{_variableCounter++}";
    }
}