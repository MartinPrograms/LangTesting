using Lexer.Lexing;
using Lexer.Parsing;
using Settings;
using StupidSimpleLogger;

namespace Lexer;

public class Lexer
{
    public LoadResult LoadResult { get; set; }
    public LexerOutput LexerOutput { get; set; } = new();
    public LanguageSettings LanguageSettings { get; set; }
    
    public Lexer(LoadResult loadResult, LanguageSettings languageSettings)
    {
        LoadResult = loadResult;
        LanguageSettings = languageSettings;
    }

    public void Lex()
    {
        var lines = LoadResult.Lines;

        for (int line = 0; line < lines.Length; line++)
        {
            var currentLine = lines[line];
            
            ScanTokens(currentLine, line);
        }
    }

    private void ScanTokens(string currentLine, int line)
    {
        var currentColumn = 0;
        
        while (currentColumn < currentLine.Length)
        {
            var currentChar = currentLine[currentColumn];
            
            if (char.IsWhiteSpace(currentChar) || currentChar == '\t' || currentChar == '\r' || currentChar == '\n')
            {
                currentColumn++;
                continue;
            }
            
            if (LanguageSettings.Operators.ContainsKey(currentChar.ToString()))
            {
                var token = ScanOperator(currentLine, ref currentColumn, line);
                if (token != null)
                {
                    AddToken(token);
                }
                continue;
            }
            
            if (LanguageSettings.StringDelimiter == currentChar)
            {
                ScanString(currentLine, ref currentColumn, line);
                continue;
            }
            
            if (char.IsDigit(currentChar))
            {
                ScanNumber(currentLine, ref currentColumn, line);
                continue;
            }
            
            if (char.IsLetter(currentChar))
            {
                ScanWord(currentLine, ref currentColumn, line);
                continue;
            }

            AddError(new Token(currentChar.ToString(), LanguageSettings.TokenTypeValue.Error, LanguageSettings.TokenTypeCategory.Identifier, line, currentColumn, 1));
            currentColumn++;
        }
    }

    private void ScanWord(string currentLine, ref int currentColumn, int line)
    {
        var startColumn = currentColumn;
        currentColumn++;
        
        while (currentColumn < currentLine.Length)
        {
            var currentChar = currentLine[currentColumn];
            
            if (!char.IsLetterOrDigit(currentChar) && currentChar != '_')
            {
                var word = currentLine.Substring(startColumn, currentColumn - startColumn);
                
                if (LanguageSettings.Keywords.TryGetValue(word, out var keyword))
                {
                    var tokenz = new Token(word, keyword, LanguageSettings.TokenTypeCategory.Literal, line, startColumn, currentColumn - startColumn);
                    AddToken(tokenz);
                    return;
                }
                
                var token = new Token(word, LanguageSettings.TokenTypeValue.Identifier, LanguageSettings.TokenTypeCategory.Identifier, line, startColumn, currentColumn - startColumn);
                AddToken(token);
                return;
            }

            currentColumn++;
        }

        AddError(new Token(currentLine.Substring(startColumn), LanguageSettings.TokenTypeValue.Error, LanguageSettings.TokenTypeCategory.Identifier, line, startColumn, currentColumn - startColumn));
    }

    private void ScanNumber(string currentLine, ref int currentColumn, int line)
    {
        var startColumn = currentColumn;
        currentColumn++;
        
        while (currentColumn < currentLine.Length)
        {
            var currentChar = currentLine[currentColumn];
            
            if (!char.IsDigit(currentChar) && currentChar != '.')
            {
                var token = new Token(currentLine.Substring(startColumn, currentColumn - startColumn), LanguageSettings.TokenTypeValue.Number, LanguageSettings.TokenTypeCategory.Literal, line, startColumn, currentColumn - startColumn);
                AddToken(token);
                return;
            }

            currentColumn++;
        }

        AddError(new Token(currentLine.Substring(startColumn), LanguageSettings.TokenTypeValue.Error, LanguageSettings.TokenTypeCategory.Identifier, line, startColumn, currentColumn - startColumn));
    }

    private void ScanString(string currentLine, ref int currentColumn, int line)
    {
        var startColumn = currentColumn;
        currentColumn++;
        
        while (currentColumn < currentLine.Length)
        {
            var currentChar = currentLine[currentColumn];
            
            if (currentChar == LanguageSettings.StringDelimiter)
            {
                var token = new Token(currentLine.Substring(startColumn, currentColumn - startColumn + 1), LanguageSettings.TokenTypeValue.String, LanguageSettings.TokenTypeCategory.Literal, line, startColumn, currentColumn - startColumn + 1);
                AddToken(token);
                currentColumn++;
                return;
            }

            currentColumn++;
        }

        AddError(new Token(currentLine.Substring(startColumn), LanguageSettings.TokenTypeValue.Error, LanguageSettings.TokenTypeCategory.Identifier, line, startColumn, currentColumn - startColumn));
    }

    private Token? ScanOperator(string currentLine, ref int currentColumn, int line)
    {
        var currentChar = currentLine[currentColumn];
        var peekedChar = PeekChar(currentLine, currentColumn);

        if (LanguageSettings.Operators.ContainsKey(currentChar.ToString() +
                                                   peekedChar)) // this also prioritizes // over /, which is what we want
        {
            var st = LanguageSettings.Operators[currentChar.ToString() + peekedChar];
            var category = LanguageSettings.Categories[st];
            if (st == LanguageSettings.TokenTypeValue.Comment)
            {
                currentColumn = currentLine.Length;
                Logger.Info("Lexer","Skipping comment");
                return null;
            }
            else
            {
                var token = new Token(currentChar.ToString() + peekedChar, st, category, line, currentColumn, 2);
                currentColumn += 2;
                return token;
            }
        }

        var ltoken = new Token(currentChar.ToString(), LanguageSettings.Operators[currentChar.ToString()], LanguageSettings.Categories[LanguageSettings.Operators[currentChar.ToString()]], line,
            currentColumn, 1);
        currentColumn++;
        return ltoken;
    }

    private char PeekChar(string currentLine, int currentColumn)
    {
        if (currentColumn + 1 >= currentLine.Length)
        {
            return '\0';
        }

        return currentLine[currentColumn + 1];
    }

    public void AddToken(Token token)
    {
        Logger.Info("Lexer",$"Adding token: {token}");
        LexerOutput.AddToken(token);
    }

    public void AddError(Token error)
    {
        Logger.Error("Lexer",$"Adding error: {error}");
        LexerOutput.AddError(error);
    }
}