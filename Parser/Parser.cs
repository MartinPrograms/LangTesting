using Lexer.Lexing;
using Settings;
using StupidSimpleLogger;

namespace Parser;


public class Parser
{
    public LexerOutput Output { get; set; }
    public ParserOutput ParserOutput { get; set; } = new();
    
    public Parser(LexerOutput output)
    {
        Output = output;
    }

    public void Parse()
    {
        var stack = new Stack<AstNode>();
        var current = new AstNode(NodeType.Root, "", 0, 0);
        stack.Push(current);
        
        Logger.Info("Parser", "Parsing tokens into AST");
        Logger.Info("Parser", $"Tokens: {Output.Tokens.Count}");
        
        List<AstNode> nodes = new();
        foreach (var token in Output.Tokens)
        {
            switch (token.TypeCategory)
            {
                case LanguageSettings.TokenTypeCategory.Keyword:
                    var keyword = new AstNode(NodeType.Keyword, token.Value, token.Line, token.Column);
                    keyword.Parent = current;
                    nodes.Add(keyword);
                    
                    Logger.Info("Parser", $"Keyword: {token.Value}");
                    
                    break;
                
                case LanguageSettings.TokenTypeCategory.Identifier:
                    var identifier = new AstNode(NodeType.Identifier, token.Value, token.Line, token.Column);
                    identifier.Parent = current;
                    nodes.Add(identifier);
                    
                    Logger.Info("Parser", $"Identifier: {token.Value}");
                    
                    break;
                
                case LanguageSettings.TokenTypeCategory.Literal:
                    var literal = new AstNode(NodeType.Literal, token.Value, token.Line, token.Column);
                    literal.Parent = current;
                    nodes.Add(literal);
                    
                    Logger.Info("Parser", $"Literal: {token.Value}");
                    
                    break;
                
                case LanguageSettings.TokenTypeCategory.OpenBracket:
                    var expression = new AstNode(NodeType.Expression, "", token.Line, token.Column);
                    expression.Parent = current;
                    nodes.Add(expression);
                    stack.Peek().Children.Add(nodes);
                    nodes = new();
                    stack.Push(expression);
                    
                    Logger.Info("Parser", "Open bracket");
                    
                    break;
                
                case LanguageSettings.TokenTypeCategory.CloseBracket:
                    if (stack.Count > 1)
                    {
                        stack.Pop();
                    }
                    else
                    {
                        ParserOutput.Errors.Add(new AstError
                        {
                            Message = "Unexpected close bracket",
                            Line = token.Line,
                            Column = token.Column
                        });
                    }

                    Logger.Info("Parser", "Close bracket");
                    
                    break;
                
                case LanguageSettings.TokenTypeCategory.Operator:
                    var @operator = new AstNode(NodeType.Operator, token.Value, token.Line, token.Column);
                    @operator.Parent = current;
                    nodes.Add(@operator);
                    
                    Logger.Info("Parser", $"Operator: {token.Value}");
                    
                    break;
                
                case LanguageSettings.TokenTypeCategory.EndOfStatement:
                    stack.Peek().Children.Add(nodes);
                    nodes = new();
                    
                    Logger.Info("Parser", "End of statement");

                    break;
                
                default:
                    ParserOutput.Errors.Add(new AstError
                    {
                        Message = "Unexpected token",
                        Line = token.Line,
                        Column = token.Column
                    });
                    
                    Logger.Error("Parser", "Unexpected token");
                    
                    break;
                    
            }
        }
        
        if (stack.Count > 1)
        {
            ParserOutput.Errors.Add(new AstError
            {
                Message = "Unexpected end of file",
                Line = Output.Tokens[^1].Line,
                Column = Output.Tokens[^1].Column
            });
        }
        
        ParserOutput.Root = current;
        Logger.Info("Parser", "Finished parsing tokens into AST");
    }
}