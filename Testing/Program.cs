using Lexer;
using Settings;
using StupidSimpleLogger;

Logger.Init();

Logger.Info("Lexer","Starting lexer...");

string inputFileName = "test.lang";

if (args.Length > 0)
{
    inputFileName = args[0];
}

Logger.Info("Parsing file",$"Reading file: {inputFileName}");

var file = Loader.ReadFile(inputFileName);

var settings = new LanguageSettings();
var lexer = new Lexer.Lexer(file, settings);
lexer.Lex();
var output = lexer.LexerOutput;

if (output.Errors.Count == 0)
{
    Logger.Info("Lexer","No errors found");
}
else
{
    Logger.Error("Lexer","Errors found");
    foreach (var error in output.Errors)
    {
        Logger.Error("Lexer",error.Value);
    }
    
    Logger.DumpLogs();
    throw new Exception("Errors found");
}

// Now that we have lexed the file, we can dump the logs
Logger.DumpLogs();

// And move on to the parser
Logger.Info("Parser","Starting parser...");

var parser = new Parser.Parser(output);
parser.Parse();

Logger.Info("Parser","Parser finished");

if (parser.ParserOutput.Errors.Count == 0)
{
    Logger.Info("Parser","No errors found");
}
else
{
    Logger.Error("Parser","Errors found");
    foreach (var error in parser.ParserOutput.Errors)
    {
        Logger.Error("Parser",error.ToString());
    }
    
    Logger.DumpLogs();
    throw new Exception("Errors found");
}

// Now that we have parsed the file, we can dump the logs
Logger.DumpLogs();

// And move on to the IR generator
Logger.Info("IR Generator","Starting IR generation...");
    
var irGenerator = new IRGenerator.IRGenerator(parser.ParserOutput, inputFileName + ".ir");
irGenerator.Generate();

Logger.Info("IR Generator","IR generation finished");

if (irGenerator.IROutput.Errors.Count == 0)
{
    Logger.Info("IR Generator","No errors found");
}
else
{
    Logger.Error("IR Generator","Errors found");
    foreach (var error in irGenerator.IROutput.Errors)
    {
        Logger.Error("IR Generator",error.ToString());
    }
    
    Logger.DumpLogs();
    throw new Exception("Errors found");
}

// Now that we have generated the IR, we can dump the logs
Logger.DumpLogs();

// And move on to the compiler
Logger.Info("Compiler","Starting compilation...");
/*
var compiler = new Compiler.Compiler(irGenerator.IRGeneratorOutput);
compiler.Compile();

Logger.Info("Compiler","Compilation finished");
*/