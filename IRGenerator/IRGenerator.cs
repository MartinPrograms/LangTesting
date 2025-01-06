using System.Diagnostics;
using Parser;
using StupidSimpleLogger;

namespace IRGenerator;

public class IRGenerator
{
    public ParserOutput ParserOutput { get; set; }
    public string InputFileName { get; set; }
    public IROutput IROutput { get; set; } = new();
    
    private bool _canGenerate = true;
    public IRGenerator(ParserOutput parserParserOutput, string inputFileName)
    {
        ParserOutput = parserParserOutput;
        InputFileName = inputFileName;
    }

    public void Generate()
    {
        if (_canGenerate)
        {
            // Generate IR, using LLVM
            var generator = new Generator(ParserOutput);
            
            IROutput.Builder = generator.Generate();
            
            Logger.Info("IRGenerator", "IR generated successfully");
            Logger.Info("IRGenerator", IROutput.Builder.Build());
            
            IROutput.Errors.AddRange(generator.Errors);
        }
    }
}