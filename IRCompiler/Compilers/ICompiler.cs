using IRGenerator.Builder;

namespace IRCompiler.Compilers;

public abstract class Compiler
{
    public string FileName { get; set; }
    public IRBuilder Builder { get; set; }
    
    public Compiler(IRBuilder builder,string fileName)
    {
        Builder = builder;
        FileName = fileName;
    }
    
    public abstract void Compile();
}