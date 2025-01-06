using IRGenerator.Builder;

namespace IRCompiler.Compilers;

public class LinuxCompiler : Compiler
{
    public LinuxCompiler(IRBuilder builder, string fileName) : base(builder, fileName)
    {
    }

    public override void Compile()
    {
        throw new NotImplementedException();
    }
}