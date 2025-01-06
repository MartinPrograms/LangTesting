using IRGenerator.Builder;

namespace IRGenerator;

public class IROutput
{
    public List<IRError> Errors { get; set; } = new();
    public IRBuilder Builder { get; set; } 
}

public class IRError
{
    public string Message { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }
    public string FileName { get; set; }
    
    public IRError(string message, int line, int column, string fileName)
    {
        Message = message;
        Line = line;
        Column = column;
        FileName = fileName;
    }
    
    public override string ToString()
    {
        return $"Error: {Message} Line: {Line} Column: {Column} File: {FileName}";
    }
}