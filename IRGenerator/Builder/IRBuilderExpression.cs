using Parser;

namespace IRGenerator.Builder;

public class IRBuilderExpression
{
    public List<AstNode> Rhs { get; set; }
    public IRBuilder Builder { get; set; }
    public string Name { get; set; }
    
    public IRBuilderExpression(List<AstNode> rhs, IRBuilder builder)
    {
        Rhs = rhs;
        Builder = builder;
        
        Name = Builder.GetNextVariableName();
    }


    public List<IRInstruction> Generate(IRType getIrType)
    {
        var instructions = new List<IRInstruction>();
        for (int i = 0; i < Rhs.Count; i++)
        {
            var inst = Rhs[i];
            if (inst.Type == NodeType.Operator)
            {
                if (inst.Value == "+")
                {
                    var lhs = Rhs[i - 1];
                    var rhs = Rhs[i + 1];
                    instructions.Add(new IRInstruction(IROpCode.Alloc, [Name, getIrType.ToString()]));
                    instructions.Add(new IRInstruction(IROpCode.Add, [Name, lhs.Value, rhs.Value]));
                }
                else if (inst.Value == "-")
                {
                    var lhs = Rhs[i - 1];
                    var rhs = Rhs[i + 1];
                    instructions.Add(new IRInstruction(IROpCode.Alloc, [Name, getIrType.ToString()]));
                    instructions.Add(new IRInstruction(IROpCode.Sub, [Name, lhs.Value, rhs.Value]));
                }
                else if (inst.Value == "*")
                {
                    var lhs = Rhs[i - 1];
                    var rhs = Rhs[i + 1];
                    instructions.Add(new IRInstruction(IROpCode.Alloc, [Name, getIrType.ToString()]));
                    instructions.Add(new IRInstruction(IROpCode.Mul, [Name, lhs.Value, rhs.Value]));
                }
                else if (inst.Value == "/")
                {
                    var lhs = Rhs[i - 1];
                    var rhs = Rhs[i + 1];
                    instructions.Add(new IRInstruction(IROpCode.Alloc, [Name, getIrType.ToString()]));
                    instructions.Add(new IRInstruction(IROpCode.Div, [Name, lhs.Value, rhs.Value]));
                }
                
                i++;
            }
        }
        return instructions;
    }
}