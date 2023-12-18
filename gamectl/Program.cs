using System.CommandLine;
using System.Diagnostics;
using gamectl;
using gamectl.Kde;

const int DefaultTdp = 6;
const string DefaultEpp = "power";

const int PluggedInTdp = 20;
const string PluggedInEpp = "balance_performance";

Console.WriteLine(Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP"));

if (Libc.GetEffectiveUserId() != 0 && !Debugger.IsAttached)
{
    Console.WriteLine("This application must be run suid root");
    return;
}

var dconfig = KscreenDoctor.GetDisplayConfiguration();

var eOption = new Option<string?>("-e", "Energy Performance Preference");
var tOption = new Option<int?>("-t", "TDP (in W)");
var gOption = new Option<int?>("-g", "Enable Gamescope scaling with specified FPS");
var mOption = new Option<string?>("-m", "Set display mode with format: widthxheight@refreshRate)");
var cArgument = new Argument<string[]?>("command", () => null, "The command to run");

var rootCommand = new RootCommand();
rootCommand.AddOption(eOption);
rootCommand.AddOption(tOption);
rootCommand.AddOption(gOption);
rootCommand.AddOption(mOption);
rootCommand.AddArgument(cArgument);

rootCommand.SetHandler((e, t, g, m, c) =>
{
    if (t is not null)
        Ryzenadj.SetTdp(t.Value);
    
    if (e is not null)
        Sysfs.SetEnergyPerformancePreference(e);
    
    if (m is not null)
        DisplayMode.SetDisplayMode(m);

    if (c is null || c.Length == 0)
        return;
    
    for (var i = 0; i < c.Length; i++)
    {
        if (c[i].Contains(' '))
            c[i] = $"\"{c[i]}\"";
    }
    
    var commandString = string.Join(' ', c);
    var commandToExecute = g is not null ? Gamescope.GetGamescopeCommandLine(g.Value, commandString) : commandString;

    var startInfo = new ProcessStartInfo(
        "systemd-inhibit",
        $"""--what=idle:sleep --who=gamectl --why="Running game" -- {commandToExecute}""")
    {
        UserName = Libc.GetUserNameByUserId(Libc.GetUserId())
    };
    
    Process.Start(startInfo)!.WaitForExit();
    
    if (t is not null)
        Ryzenadj.SetTdp(DefaultTdp);
    
    if (e is not null)
        Sysfs.SetEnergyPerformancePreference(DefaultEpp);
}, eOption, tOption, gOption, mOption, cArgument);

await rootCommand.InvokeAsync(args);