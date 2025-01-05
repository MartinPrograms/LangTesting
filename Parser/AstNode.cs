namespace Parser;

public class AstNode
{
    public NodeType Type { get; set; }
    public string Value { get; set; }
    public AstNode Parent { get; set; } // Parent function or class, used for scoping
    public List<List<AstNode>> Children { get; set; } = new(); // List of lists of nodes, each list in the list are together
    
    public int Line { get; set; }
    public int Column { get; set; }
    
    public AstNode(NodeType type, string value, int line, int column)
    {
        Type = type;
        Value = value;
        Line = line;
        Column = column;
    }
    
    public override string ToString()
    {
        return $"Node: {Value} Type: {Type} Line: {Line} Column: {Column}";
    }
}

public enum NodeType
{
    Root,
    Keyword,
    Identifier,
    Operator,
    Literal,
    Expression,
    EndOfStatement
}