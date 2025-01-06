using IRGenerator.Builder;

namespace IRCompiler.Compilers;

public class MacOSCompiler : Compiler
{
    public MacOSCompiler(IRBuilder builder, string fileName) : base(builder, fileName)
    {
    }

    public override void Compile()
    {
        throw new NotImplementedException();
    }
}