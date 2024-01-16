using System.CommandLine;

namespace Gamerun;

public static class CommandLine
{
    public static RootCommand CreateRootCommand(Action<bool, string?, bool, string?, string?, int?, string[]> action)
    {
        var dOption = new Option<bool>("-d", "Run as daemon");
        var eOption = new Option<string?>("-e", "Set Energy Performance Preference");
        var gOption = new Option<bool>("-g", "Enable Gamescope");
        var mOption = new Option<string?>("-m", "Set display mode, e.g. '1920x1080@120'");
        var pOption = new Option<string?>("-p", "Park CPU cores, e.g. '4,5,6-11'");
        var tOption = new Option<int?>("-t", "Set TDP (in Watts)");
        
        var cArgument = new Argument<string[]>("command", "The command to run");

        var rootCommand = new RootCommand();
        rootCommand.AddOption(dOption);
        rootCommand.AddOption(eOption);
        rootCommand.AddOption(tOption);
        rootCommand.AddOption(gOption);
        rootCommand.AddOption(mOption);
        rootCommand.AddOption(pOption);
        rootCommand.AddArgument(cArgument);
        
        rootCommand.SetHandler(action, dOption, eOption, gOption, mOption, pOption, tOption, cArgument);

        return rootCommand;
    }
}