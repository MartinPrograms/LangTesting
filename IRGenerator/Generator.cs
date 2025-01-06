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
    
    public IRBuilder Generate()
    {
        IRPlatform targetPlatform = IRPlatform.Win64;
        if (OperatingSystem.IsMacOS())
        {
            targetPlatform = IRPlatform.MacOS64;
        }
        else if (OperatingSystem.IsLinux())
        {
            targetPlatform = IRPlatform.Linux64;
        }
        
        // If you're still running 32-bit, you should upgrade lol
        
        var builder = new IRBuilder("main", targetPlatform);
        
        GenerateNode(_parser.Root, builder);

        return builder;
    }
    
    private void GenerateNode(AstNode parserRoot, IRBuilder builder, IRFunction? currentFunction = null)
    {
        Logger.Info("IRGenerator", $"Generating node: {parserRoot.Type}");

        // A child in the child list is a List<AstNode>. So parserRoot.Children is a List<List<AstNode>>, this is done to group nodes together, seperated by either ; or {} or ()

        FunctionPass(parserRoot, builder);
        
        foreach (var childList in parserRoot.Children)
        {
            if (childList.Count == 0)
            {
                continue;
            }
            
            var firstChild = childList.First(); // The keyword, variable name, literal, etc.
            
            if (firstChild.Children.Count > 0)
            {
                GenerateNode(firstChild, builder, currentFunction);
            }
            else
            {
                if (firstChild.Value == "fn")
                {
                    currentFunction = builder.GetFunction(childList[2].Value);
                }
                
                if (IRTypeHelper.GetIRType(firstChild.Value) != IRType.Unknown)
                {
                    LiteralCreation(parserRoot, builder, currentFunction, firstChild, childList);
                }

                if (firstChild.Value == "return")
                {
                    if (currentFunction != null)
                    {
                        if (childList.Count == 2)
                        {
                            var returnValue = childList[1].Value;
                            currentFunction.Instructions.Add(new IRInstruction(IROpCode.Ret, [returnValue]));
                        }
                        else
                        {
                            currentFunction.Instructions.Add(new IRInstruction(IROpCode.Ret, []));
                        }
                    }
                    else
                    {
                        Logger.Error("IRGenerator", "Return statement outside of function");
                        Errors.Add(new IRError("Return statement outside of function", parserRoot.Line,
                            parserRoot.Column, "IRGenerator"));
                    }
                }

                if (builder.HasFunction(firstChild.Value))
                {
                    // We call using the function name, and like so: call function_name _ arg1 ... argn, the _ is the return value, a _ is used to indicate that it should be discarded
                    var function = builder.GetFunction(firstChild.Value);
                    var instruction = new IRInstruction(IROpCode.Call, [function.Name, "_"]); // TODO: Implement function arguments
                    currentFunction?.Instructions.Add(instruction);
                    
                    Logger.Info("IRGenerator", $"Function call: {function.Name}");
                }

                if (currentFunction != null && currentFunction.Locals.Any(x => x.Name == firstChild.Value) && childList.Count > 2 && childList[1].Value == "=")
                {
                    // We are assigning a value to a variable
                    var variable = currentFunction.Locals.First(x => x.Name == firstChild.Value);
                    var rhs = childList.Skip(2).ToList();
                    var expression = new IRBuilderExpression(rhs, builder);
                    List<IRInstruction> instructions = expression.Generate(variable.Type);
                    var instruction = new IRInstruction(IROpCode.Store, [expression.Name, variable.Name]);
                    currentFunction.Instructions.AddRange(instructions);
                    currentFunction.Instructions.Add(instruction);
                }
            }
        }
    }

    private void FunctionPass(AstNode parserRoot, IRBuilder builder)
    {
        foreach (var childList in parserRoot.Children)
        {
            if (childList.Count == 0)
            {
                continue;
            }

            var firstChild = childList.First(); // The keyword, variable name, literal, etc.

            if (firstChild.Children.Count > 0)
            {
                // No functions in functions, only global functions >:(
            }
            else
            {
                if (firstChild.Value == "fn") // fn void main() {}
                {
                    var functionType = childList[1].Value; // The first child is the function type
                    var functionName = childList[2].Value; // The second child is the function name
                    var
                        functionArgs =
                            childList
                                [3]; // The third child is the function arguments, this is an expression with the arguments, does not actually work yet... // TODO: Implement function arguments

                    Logger.Info("IRGenerator", $"Function: {functionType} {functionName}");

                    builder.AddFunction(functionName, IRTypeHelper.GetIRType(functionType), new List<IRVariable>());

                    var function = builder.GetFunction(functionName);

                    function.Instructions.Add(new IRInstruction(IROpCode.Label, [functionName]));
                }
            }
        }
    }

    private void LiteralCreation(AstNode parserRoot, IRBuilder builder, IRFunction? currentFunction, AstNode firstChild,
        List<AstNode> childList)
    {
        var variableType = firstChild.Value;
        var variableName = childList[1].Value;

        var irvariable = new IRVariable(variableName, IRTypeHelper.GetIRType(variableType),
            "");
                    
        if (childList.Count == 4 || childList.Count == 5 && childList[2].Value == "=")
        {
            var variableValue = childList[3].Value;
            if (builder.HasGlobal(variableValue) && currentFunction != null)
            {
                // We need to load the global variable
                var variable = builder.GetGlobal(variableValue);
                var instruction = new IRInstruction(IROpCode.Store, [variable.Name, irvariable.Name]);
                currentFunction.Locals.Add(irvariable);
                currentFunction.Instructions.Add(instruction);
            }
            else if (builder.HasFunction(variableValue) && currentFunction != null)
            {
                // We need to call the function
                var function = builder.GetFunction(variableValue);
                var type = function.ReturnType;
                if (type != irvariable.Type)
                {
                    Logger.Error("IRGenerator", "Function return type does not match variable type, expected: " + irvariable.Type + " got: " + type);
                    Errors.Add(new IRError("Function return type does not match variable type, expected: " + irvariable.Type + " got: " + type, parserRoot.Line,
                        parserRoot.Column, "IRGenerator"));
                }
                else
                {
                    var instruction =
                        new IRInstruction(IROpCode.Call,
                        [
                            function.Name, irvariable.Name
                        ]); // after 2nd argument, we need to add the arguments, but we don't have that yet // TODO: Implement function arguments
                    currentFunction.Locals.Add(irvariable);
                    currentFunction.Instructions.Add(instruction);
                }
            }
            else
            if (currentFunction != null)
            {
                irvariable.Value = variableValue;
                var instruction = irvariable.DefaultStoreInstruction();
                currentFunction.Locals.Add(irvariable);
                currentFunction.Instructions.Add(instruction); // Add the store instruction, not needed for globals they're going to be pre-stored by the compiler
            }
            else
            {
                if (!builder.HasGlobal(variableName) && !builder.HasFunction(variableName))
                {
                    builder.AddGlobal(variableName, IRTypeHelper.GetIRType(variableType), variableValue);
                }
                else
                {
                    Logger.Error("IRGenerator", "Variable already exists or function call as global");
                    Errors.Add(new IRError("Variable already exists or function call as global", parserRoot.Line, parserRoot.Column, "IRGenerator"));
                }
            }
        }
        else
        {
            if (childList.Count > 2)
            {
                // this is an expression
                Logger.Info("IRGenerator", "Expression");
                            
                var rhs = childList.Skip(2).ToList();
                var expression = new IRBuilderExpression(rhs, builder);
                List<IRInstruction> instructions = expression.Generate(IRTypeHelper.GetIRType(variableType));
                var instruction = new IRInstruction(IROpCode.Store, [expression.Name, irvariable.Name]);
                currentFunction?.Locals.Add(irvariable);
                currentFunction?.Instructions.AddRange(instructions);
                currentFunction?.Instructions.Add(instruction);
            }
            else
            {
                Logger.Error("IRGenerator", "Invalid variable declaration");
                Errors.Add(new IRError("Invalid variable declaration", parserRoot.Line, parserRoot.Column, "IRGenerator"));
            }
        }
    }
}