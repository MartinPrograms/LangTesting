namespace IRGenerator.Builder;

public class IRInstruction
{
    public IROpCode OpCode { get; set; }
    public string[] Arguments { get; set; }
    
    public IRInstruction(IROpCode opCode, string[] arguments)
    {
        OpCode = opCode;
        Arguments = arguments;
    }
    
    public override string ToString()
    {
        return $"{OpCode} {string.Join(" ", Arguments)}";
    }
}

public enum IROpCode
{
    Label, // For marking a label
    Add,
    Sub,
    Mul,
    Div,
    
    And,
    Or,
    Xor,
    Shl,
    Shr,
    
    Load,
    Store,
    Alloc,
    
    Br,  // Branch
    Bz,  // Branch if zero
    Bnz, // Branch if not zero
    Call,
    Phi,
    Select,
    
    Cmp, // Compare
    Icmp, // Integer compare
    Fcmp, // Float compare
    
    Gep, // Get element pointer
    ExtractElement,
    InsertElement,
    ShuffleVector,
    
    Ret,
    
    Trunc, // Truncate
    Zext, // Zero extend
    Sext, // Sign extend
}