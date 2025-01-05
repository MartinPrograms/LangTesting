namespace IRGenerator.Builder;

public class IRPlatform
{
    public IRTarget Target { get; set; }
    public IRArchitecture Architecture { get; set; }
    public string OutputPath { get; set; }
    public string OutputName { get; set; }
    
    public static IRPlatform Win64 => new(IRTarget.Windows, IRArchitecture.X64, "bin", "output");
    public static IRPlatform Linux64 => new(IRTarget.Linux, IRArchitecture.X64, "bin", "output");
    public static IRPlatform MacOS64 => new(IRTarget.MacOS, IRArchitecture.X64, "bin", "output");
    public static IRPlatform FreeBSD64 => new(IRTarget.FreeBSD, IRArchitecture.X64, "bin", "output");
    public static IRPlatform AndroidARM => new(IRTarget.Android, IRArchitecture.ARM, "bin", "output");
    public static IRPlatform AndroidARM64 => new(IRTarget.Android, IRArchitecture.ARM64, "bin", "output");
    public static IRPlatform IOS => new(IRTarget.IOS, IRArchitecture.ARM64, "bin", "output");
    public static IRPlatform WebAssembly => new(IRTarget.WebAssembly, IRArchitecture.WebAssembly, "bin", "output");
    
    public IRPlatform(IRTarget target, IRArchitecture architecture, string outputPath, string outputName)
    {
        Target = target;
        Architecture = architecture;
        OutputPath = outputPath;
        OutputName = outputName;
    }
    
    public override string ToString()
    {
        return $"{Target} {Architecture} : \'{OutputPath}\' \'{OutputName}\'";
    }
}

public enum IRTarget
{
    Windows,
    Linux,
    MacOS,
    FreeBSD,
    Android,
    IOS,
    WebAssembly
}

public enum IRArchitecture
{
    X86,
    X64,
    ARM,
    ARM64,
    WebAssembly
}