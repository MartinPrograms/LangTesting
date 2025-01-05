using System.Reflection;

namespace Settings;

public class LanguageSettings
{
    // Shamelessly stolen from Crafting Interpreters
    public enum TokenTypeValue
    {
        LeftParenthesis,
        RightParenthesis,
        LeftBrace,
        RightBrace,
        LeftBracket,
        RightBracket,
        Comma,
        Dot,
        Minus,
        Plus,
        Semicolon,
        Slash,
        Star,
        Comment,

        Bang,
        BangEqual,
        Equal,
        EqualEqual,
        Greater,
        GreaterEqual,
        Less,
        LessEqual,

        Identifier,
        String,
        Number,

        And,
        Class,
        Else,
        False,
        Fun,
        For,
        If,
        Null,
        Or,
        Return,
        This,
        True,
        Var,
        While,

        Error,
        Eof
    }

    public enum TokenTypeCategory
    {
        Keyword,
        Identifier,
        Literal,
        OpenBracket,
        CloseBracket,
        Operator,
        EndOfStatement
    }

    public Dictionary<string, TokenTypeValue> Operators { get; set; } = new()
    {
        { "(", TokenTypeValue.LeftParenthesis },
        { ")", TokenTypeValue.RightParenthesis },
        { "{", TokenTypeValue.LeftBrace },
        { "}", TokenTypeValue.RightBrace },
        { "[", TokenTypeValue.LeftBracket },
        { "]", TokenTypeValue.RightBracket },
        { ",", TokenTypeValue.Comma },
        { ".", TokenTypeValue.Dot },
        { "-", TokenTypeValue.Minus },
        { "+", TokenTypeValue.Plus },
        { ";", TokenTypeValue.Semicolon },
        { "//", TokenTypeValue.Comment },
        { "/", TokenTypeValue.Slash },
        { "*", TokenTypeValue.Star },

        { "!", TokenTypeValue.Bang },
        { "!=", TokenTypeValue.BangEqual },
        { "=", TokenTypeValue.Equal },
        { "==", TokenTypeValue.EqualEqual },
        { ">", TokenTypeValue.Greater },
        { ">=", TokenTypeValue.GreaterEqual },
        { "<", TokenTypeValue.Less },
        { "<=", TokenTypeValue.LessEqual }
    };

    public Dictionary<string, TokenTypeValue> Keywords { get; set; } = new()
    {
        { "and", TokenTypeValue.And },
        { "class", TokenTypeValue.Class },
        { "else", TokenTypeValue.Else },
        { "false", TokenTypeValue.False },
        { "fn", TokenTypeValue.Fun }, 
        { "for", TokenTypeValue.For },
        { "if", TokenTypeValue.If },
        { "null", TokenTypeValue.Null },
        { "or", TokenTypeValue.Or },
        { "return", TokenTypeValue.Return },
        { "this", TokenTypeValue.This },
        { "true", TokenTypeValue.True },
        { "var", TokenTypeValue.Var },
        { "while", TokenTypeValue.While }
    };

    public char StringDelimiter { get; set; } = '"';

    public Dictionary<TokenTypeValue, TokenTypeCategory> Categories { get; set; } = new()
    {
        { TokenTypeValue.LeftParenthesis, TokenTypeCategory.OpenBracket },
        { TokenTypeValue.RightParenthesis, TokenTypeCategory.CloseBracket },
        { TokenTypeValue.LeftBrace, TokenTypeCategory.OpenBracket },
        { TokenTypeValue.RightBrace, TokenTypeCategory.CloseBracket },
        { TokenTypeValue.LeftBracket, TokenTypeCategory.OpenBracket },
        { TokenTypeValue.RightBracket, TokenTypeCategory.CloseBracket },
        { TokenTypeValue.Comma, TokenTypeCategory.Operator },
        { TokenTypeValue.Dot, TokenTypeCategory.Operator },
        { TokenTypeValue.Minus, TokenTypeCategory.Operator },
        { TokenTypeValue.Plus, TokenTypeCategory.Operator },
        { TokenTypeValue.Semicolon, TokenTypeCategory.EndOfStatement },
        { TokenTypeValue.Slash, TokenTypeCategory.Operator },
        { TokenTypeValue.Star, TokenTypeCategory.Operator },
        { TokenTypeValue.Comment, TokenTypeCategory.Operator },
        { TokenTypeValue.Bang, TokenTypeCategory.Operator },
        { TokenTypeValue.BangEqual, TokenTypeCategory.Operator },
        { TokenTypeValue.Equal, TokenTypeCategory.Operator },
        { TokenTypeValue.EqualEqual, TokenTypeCategory.Operator },
        { TokenTypeValue.Greater, TokenTypeCategory.Operator },
        { TokenTypeValue.GreaterEqual, TokenTypeCategory.Operator },
        { TokenTypeValue.Less, TokenTypeCategory.Operator },
        { TokenTypeValue.LessEqual, TokenTypeCategory.Operator },
        { TokenTypeValue.Identifier, TokenTypeCategory.Identifier },
        { TokenTypeValue.String, TokenTypeCategory.Literal },
        { TokenTypeValue.Number, TokenTypeCategory.Literal },
        { TokenTypeValue.And, TokenTypeCategory.Keyword },
        { TokenTypeValue.Class, TokenTypeCategory.Keyword },
        { TokenTypeValue.Else, TokenTypeCategory.Keyword },
        { TokenTypeValue.False, TokenTypeCategory.Keyword },
        { TokenTypeValue.Fun, TokenTypeCategory.Keyword },
        { TokenTypeValue.For, TokenTypeCategory.Keyword },
        { TokenTypeValue.If, TokenTypeCategory.Keyword },
        { TokenTypeValue.Null, TokenTypeCategory.Keyword },
        { TokenTypeValue.Or, TokenTypeCategory.Keyword },
        { TokenTypeValue.Return, TokenTypeCategory.Keyword },
        { TokenTypeValue.This, TokenTypeCategory.Keyword },
        { TokenTypeValue.True, TokenTypeCategory.Keyword },
        { TokenTypeValue.Var, TokenTypeCategory.Keyword },
        { TokenTypeValue.While, TokenTypeCategory.Keyword },
        { TokenTypeValue.Error, TokenTypeCategory.Identifier },
        { TokenTypeValue.Eof, TokenTypeCategory.Identifier }
    };
}