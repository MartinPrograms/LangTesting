using Lexer.Parsing;

namespace Lexer;

public class Loader
{
    public static LoadResult ReadFile(string inputFileName)
    {
        var output = new LoadResult(Path.GetFullPath(inputFileName), File.ReadAllLines(inputFileName), File.ReadAllText(inputFileName));
        return output;
    }
}