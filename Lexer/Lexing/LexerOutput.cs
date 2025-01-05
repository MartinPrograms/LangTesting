namespace Lexer.Lexing;

public class LexerOutput
{
    public List<Token> Tokens { get; set; } = new();
    public List<Token> Errors { get; set; } = new();
    
    public void AddToken(Token token)
    {
        Tokens.Add(token);
    }
    
    public void AddError(Token error)
    {
        Errors.Add(error);
    }
}