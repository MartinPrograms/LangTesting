namespace Parser;

public class AstError
{
    public string Message { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }
    
    public AstError(string message, int line, int column)
    {
        Message = message;
        Line = line;
        Column = column;
    }
    
    public override string ToString()
    {
        return $"Error: {Message} Line: {Line} Column: {Column}";
    }

    public AstError()
    {
        
    }
}