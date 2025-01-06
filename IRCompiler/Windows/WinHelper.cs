using System.Text;
using IRGenerator.Builder;

namespace IRCompiler.Windows;

public class WinHelper
{
    public static string GlobalVariableToAsm(IRVariable var)
    {
        return $"{var.Name}: {WinHelper.AsmTypeFromIRType(var.Type)} {var.Value}";
    }

    private static WinAsmType AsmTypeFromIRType(IRType varType)
    {
        switch (varType)
        {
            case IRType.Int:
            case IRType.Pointer: 
                return WinAsmType.dq;
            case IRType.Float:
                return WinAsmType.dd; 
            case IRType.String:
                return WinAsmType.dq; 
            case IRType.Bool:
                return WinAsmType.db; 
            case IRType.Void:
                throw new InvalidOperationException("Void type does not map to a storage type.");
            default:
                throw new ArgumentOutOfRangeException(nameof(varType), "Unsupported IRType.");
        }
    }

    public static string FunctionToAsm(IRFunction function)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"global {function.Name}");
        foreach (var instruction in function.Instructions)
        {
            sb.AppendLine("\t" + WinHelper.InstructionToAsm(instruction));
        }
        return sb.ToString();
    }

    private static string InstructionToAsm(IRInstruction instruction)
    {
        var sb = new StringBuilder();

        if (instruction.OpCode == IROpCode.Ret)
        {
            // Move to rax
            if (instruction.Arguments.Length>0)
            {
                sb.AppendLine($"mov rax, [{instruction.Arguments[0]}]");
            }
            sb.AppendLine("\tret");
        }

        if (instruction.OpCode == IROpCode.Label)
        {
            sb.AppendLine($"{instruction.Arguments[0]}:");
        }

        return sb.ToString();
        
    }
}