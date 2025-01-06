using System.Text;
using IRCompiler.Windows;
using IRGenerator.Builder;
using StupidSimpleLogger;

namespace IRCompiler.Compilers;

public class WindowsCompiler : Compiler
{
    public WindowsCompiler(IRBuilder builder, string fileName) : base(builder, fileName)
    {
    }

    public override void Compile()
    {
        // Compile the IR to a windows compatible assembly code
        // If Builder.HasMainFunction is true, we simply link using a custom entry point

        var sb = new StringBuilder();
        
        Logger.Info("WindowsCompiler", "Compiling IR to Windows assembly code");
        
        sb.AppendLine($"; ModuleID = '{FileName}'");
        sb.AppendLine("; IR TO ASSEMBLY COMPILER : WINX64"); // We are writing bare metal assembly code
        sb.AppendLine($"; COMPILED ON {DateTime.Now}");
        sb.AppendLine();
        sb.AppendLine("section .data");
        
        foreach (var global in Builder.Globals)
        {
            sb.AppendLine(WinHelper.GlobalVariableToAsm(global));
        }
        
        sb.AppendLine();
        sb.AppendLine("section .text"); // Start of the text section (code)
        
        sb.AppendLine();
        
        sb.AppendLine($"; Functions");
        sb.AppendLine($"; Function count: {Builder.Functions.Count}");
        sb.AppendLine();

        if (Builder.HasMainFunction)
        {
            // Create a _start function
            sb.AppendLine("global _start");
            sb.AppendLine("_start:");
            sb.AppendLine("\tcall main");
            sb.AppendLine("\tmov rcx, rax"); // Store the return value in rcx
            sb.AppendLine("\tmov rax, 60"); // Exit syscall
            
            sb.AppendLine("\tsyscall");
        }
        
        foreach (var function in Builder.Functions)
        {
            sb.AppendLine($"; Function: {function.Name} - {function.ReturnType}");
            sb.AppendLine(WinHelper.FunctionToAsm(function));
        }
        
        // Write the assembly code to a file
        
        var source = sb.ToString();
        File.WriteAllText($"{FileName}", source);
        
        Logger.Info("WindowsCompiler", $"Assembly code written to {FileName}");
        Logger.Info("WindowsCompiler", source);
    }
}