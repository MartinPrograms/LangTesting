using IRCompiler.Compilers;
using IRGenerator;
using IRGenerator.Builder;
using StupidSimpleLogger;

namespace IRCompiler;

public class IRCompiler
{
    public IROutput IrOutput { get; set; }
    public string InputFileName { get; set; }
    
    public IRCompiler(IROutput irGeneratorIrOutput, string inputFileName)
    {
        IrOutput = irGeneratorIrOutput;
        InputFileName = inputFileName;
        
        Logger.Info("IRCompiler","IRCompiler initialized, output assembly will be written to " + inputFileName);
    }

    public void Compile()
    {
        // This writes assembly code, which can be compiled using clang, ld, or any other assembler
        // The assembly code is written to the input file name, with a .s extension

        var irBuilder = IrOutput.Builder;

        if (irBuilder.Platform.Target == IRTarget.Windows)
        {
            var windowsCompiler = new WindowsCompiler(irBuilder, InputFileName);
            windowsCompiler.Compile();
        }
        else if (irBuilder.Platform.Target == IRTarget.MacOS)
        {
            var macOsCompiler = new MacOSCompiler(irBuilder, InputFileName);
            macOsCompiler.Compile();
        }
        else if (irBuilder.Platform.Target == IRTarget.Linux)
        {
            var linuxCompiler = new LinuxCompiler(irBuilder, InputFileName);
            linuxCompiler.Compile();
        }
        else
        {
            throw new Exception("Unsupported target platform");
        }
    }
}