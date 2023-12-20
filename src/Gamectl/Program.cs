using System.CommandLine;
using System.Diagnostics;
using Gamectl;

if (Libc.GetEffectiveUserId() != 0 && !Debugger.IsAttached)
{
    Console.WriteLine("This application must be run suid root");
    return;
}

Configuration.LoadConfiguration();

var eOption = new Option<string?>("-e", "Set Energy Performance Preference");
var tOption = new Option<int?>("-t", "Set TDP (in Watts)");
var gOption = new Option<int?>("-g", "Enable Gamescope with specified max FPS");
var mOption = new Option<string?>("-m", "Set display mode, e.g. '1920x1080@120'");
var pOption = new Option<string?>("-p", "Park CPU cores, e.g. '4,5,6-11'");
var cArgument = new Argument<string[]>("command", "The command to run");

var rootCommand = new RootCommand();
rootCommand.AddOption(eOption);
rootCommand.AddOption(tOption);
rootCommand.AddOption(gOption);
rootCommand.AddOption(mOption);
rootCommand.AddOption(pOption);
rootCommand.AddArgument(cArgument);

rootCommand.SetHandler((e, t, g, m, p, c) =>
{
    if (c.Length == 0)
    {
        Console.WriteLine("Command argument is required.");
        return;
    }
    
    var tdp = t ?? Configuration.DefaultTdp;
    if (tdp is not null)
        Ryzenadj.SetTdp(tdp.Value);

    var epp = e ?? Configuration.DefaultEpp;
    if (epp is not null)
        Sysfs.SetEnergyPerformancePreference(epp);
    
    Sysfs.SetCpuCorePower(ParseCoreSpecification(p ?? Configuration.DefaultCoreParking), false);
    
    // Drop privileges
    Libc.SetEffectiveUserId(Libc.GetUserId());

    var mode = m ?? Configuration.DefaultMode;
    if (mode is not null)
        DisplayMode.SetDisplayMode(mode);
    
    for (var i = 0; i < c.Length; i++)
    {
        if (c[i].Contains(' '))
            c[i] = $"\"{c[i]}\"";
    }
    
    var commandString = string.Join(' ', c);
    var commandToExecute = g is not null ? Gamescope.GetGamescopeCommandLine(g.Value, commandString) : commandString;

    Process
        .Start(
            "systemd-inhibit",
            $"""--what=idle:sleep --who=gamectl --why="Running game" -- {commandToExecute}""")
        .WaitForExit();

    if (Configuration.ExitMode is not null)
        DisplayMode.SetDisplayMode(Configuration.ExitMode);
    
    // Regain privileges
    Libc.SetEffectiveUserId(0);
    
    if (Configuration.ExitCoreParking is not null)
        Sysfs.SetCpuCorePower(ParseCoreSpecification(Configuration.ExitCoreParking), true);
    
    if (Configuration.ExitEpp is not null)
        Sysfs.SetEnergyPerformancePreference(Configuration.ExitEpp);
    
    if (Configuration.ExitTdp is not null)
        Ryzenadj.SetTdp(Configuration.ExitTdp.Value);
}, eOption, tOption, gOption, mOption, pOption, cArgument);

await rootCommand.InvokeAsync(args);

int[] ParseCoreSpecification(string? coreSpecification)
{
    if (string.IsNullOrWhiteSpace(coreSpecification))
        return Array.Empty<int>();
    
    if (coreSpecification == "none")
        return Sysfs.GetCpuCores();
    
    var coreStrings = (coreSpecification ?? "")
        .Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    var cores = new List<int>();

    foreach (var coreString in coreStrings)
    {
        var coreRange = coreString
            .Split('-', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToArray();
        
        cores.AddRange(coreRange.Length == 2 ? Enumerable.Range(coreRange[0], coreRange[1] - coreRange[0] + 1) : coreRange);
    }

    return cores.ToArray();
}