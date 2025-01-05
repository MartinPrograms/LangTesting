namespace Lexer.Parsing;

public class LoadResult
{
    public string FileName { get; set; }
    public string[] Lines { get; set; }
    public string Source { get; set; }
    
    public LoadResult(string fileName, string[] lines, string source )
    {
        FileName = fileName;
        Lines = lines;
        Source = source;
    }
}