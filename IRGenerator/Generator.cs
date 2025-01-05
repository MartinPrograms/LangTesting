using System.Diagnostics;
using IRGenerator.Builder;
using Parser;
using StupidSimpleLogger;

namespace IRGenerator;

public class Generator
{
    private ParserOutput _parser;
    public List<IRError> Errors { get; set; } = new();
    public Generator(ParserOutput parser)
    {
        _parser = parser;
    }
    
    public unsafe void Generate()
    {
        var builder = new IRBuilder("main");
        
        GenerateNode(_parser.Root, builder);

        var ir = builder.Build();
        Logger.Info("IRGenerator", ir);
    }
    
    private void GenerateNode(AstNode parserRoot, IRBuilder builder, IRFunction? currentFunction = null)
    {
        Logger.Info("IRGenerator", $"Generating node: {parserRoot.Type}");
        
        // A child in the child list is a List<AstNode>. So parserRoot.Children is a List<List<AstNode>>, this is done to group nodes together, seperated by either ; or {} or ()
        foreach (var childList in parserRoot.Children)
        {
            var firstChild = childList.First(); // The keyword, variable name, literal, etc.
            
            if (firstChild.Children.Count > 0)
            {
                GenerateNode(firstChild, builder, currentFunction);
            }
            else
            {
                if (firstChild.Value == "fn") // fn void main() {}
                {
                    var functionType = childList[1].Value; // The first child is the function type
                    var functionName = childList[2].Value; // The second child is the function name
                    var functionArgs = childList[3]; // The third child is the function arguments, this is an expression with the arguments, does not actually work yet... // TODO: Implement function arguments
                    
                    Logger.Info("IRGenerator", $"Function: {functionType} {functionName}");
                    
                    builder.AddFunction(functionName, IRTypeHelper.GetIRType(functionType), new List<IRVariable>());
                    
                    var function = builder.GetFunction(functionName);
                    currentFunction = function;
                }

                if (IRTypeHelper.GetIRType(firstChild.Value) != IRType.Unknown)
                {
                    // If an = sign is the third child, then it is a variable declaration
                    if (childList.Count == 4 && childList[2].Value == "=")
                    {
                        var variableType = firstChild.Value;
                        var variableName = childList[1].Value;
                        var variableValue = childList[3].Value;
                        
                        Logger.Info("IRGenerator", $"Variable: {variableType} {variableName} = {variableValue}");

                        currentFunction!.Locals.Add(new IRVariable(variableName, IRTypeHelper.GetIRType(variableType),
                            variableValue));
                    }
                }
            }
        }
    }
}