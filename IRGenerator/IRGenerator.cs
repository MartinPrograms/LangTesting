using System.Diagnostics;
using Parser;

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
        
        // Check if LLVM is installed
        if (Process.Start("llvm-config", "--version") == null)
        {
            IROutput.Errors.Add(new IRError("LLVM is not installed", 0, 0, InputFileName));
            _canGenerate = false;
            return;
        }
    }

    public void Generate()
    {
        if (_canGenerate)
        {
            // Generate IR, using LLVM
            var generator = new Generator(ParserOutput);
            
            generator.Generate();
            
            IROutput.Errors.AddRange(generator.Errors);
        }
    }
}