namespace Parser;

public class ParserOutput
{
    public AstNode Root { get; set; }
    public List<AstError> Errors { get; set; } = new();
}