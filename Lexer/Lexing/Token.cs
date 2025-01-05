using Settings;

namespace Lexer.Lexing;

public class Token
{
    public string Value { get; set; }
    public LanguageSettings.TokenTypeValue TypeValue { get; set; }
    public LanguageSettings.TokenTypeCategory TypeCategory { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }
    public int Length { get; set; }
    
    public Token(string value, LanguageSettings.TokenTypeValue typeValue, LanguageSettings.TokenTypeCategory typeCategory, int line, int column, int length)
    {
        Value = value;
        TypeValue = typeValue;
        TypeCategory = typeCategory;
        Line = line;
        Column = column;
        Length = length;
    }
    
    public override string ToString()
    {
        return $"Token: {Value} Type: {TypeValue} Line: {Line} Column: {Column} Length: {Length}";
    }
}