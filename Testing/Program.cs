using System.Diagnostics;
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
Logger.Info("IRCompiler","Starting compilation...");

var compiler = new IRCompiler.IRCompiler(irGenerator.IROutput, inputFileName + ".s"); // Assemble the output to a file
compiler.Compile();

Logger.Info("IRCompiler","Compilation finished");

if (compiler.IrOutput.Errors.Count == 0)
{
    Logger.Info("IRCompiler","No errors found");
}
else
{
    Logger.Error("IRCompiler","Errors found");
    foreach (var error in compiler.IrOutput.Errors)
    {
        Logger.Error("IRCompiler",error.ToString());
    }
    
    Logger.DumpLogs();
    throw new Exception("Errors found");
}

// Now that we have compiled the file, we can dump the logs
Logger.DumpLogs();


if (OperatingSystem.IsWindows())
{
    // Run nasm -f win64 -o program.obj .\test.lang.s
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "nasm",
            Arguments = $"-f win64 -o {inputFileName}.obj {inputFileName}.s",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };
    
    process.Start();
    
    var outputstr = process.StandardOutput.ReadToEnd();
    var errorstr = process.StandardError.ReadToEnd();
        
    Logger.Info("NASM",outputstr);
    Logger.Error("NASM",errorstr);
    
    process.WaitForExit();
    
    // run link program.obj /subsystem:console /entry:_start /LARGEADDRESSAWARE:NO if main function exists
    string argzzz = $"/LARGEADDRESSAWARE:NO";
    if (irGenerator.IROutput.Builder.HasMainFunction)
    {
        argzzz = $"/subsystem:console /entry:_start /LARGEADDRESSAWARE:NO";
    }
    
    process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "link",
            Arguments = $"{inputFileName}.obj {argzzz}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };
    
    process.Start();
    
    outputstr = process.StandardOutput.ReadToEnd();
    errorstr = process.StandardError.ReadToEnd();
    
    Logger.Info("LINK",outputstr);
    Logger.Error("LINK",errorstr);
    
    process.WaitForExit();
    
    // run program.exe
    process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = $"{inputFileName}.exe",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };
    
    process.Start();
    
    outputstr = process.StandardOutput.ReadToEnd();
    errorstr = process.StandardError.ReadToEnd();
    
    Logger.Info("PROGRAM",outputstr);
    Logger.Error("PROGRAM",errorstr);
    
    process.WaitForExit();
    
    Logger.Info("IRCompiler","Program finished with exit code " + process.ExitCode);
    
    Logger.DumpLogs();  
}