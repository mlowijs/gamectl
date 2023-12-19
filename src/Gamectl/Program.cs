﻿using System.CommandLine;
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
var dOption = new Option<bool>("-d", "Daemon mode, for autosetting TDP on AC");
var cArgument = new Argument<string[]?>("command", () => null, "The command to run");

var rootCommand = new RootCommand();
rootCommand.AddOption(eOption);
rootCommand.AddOption(tOption);
rootCommand.AddOption(gOption);
rootCommand.AddOption(mOption);
rootCommand.AddOption(pOption);
rootCommand.AddArgument(cArgument);

rootCommand.SetHandler((e, t, g, m, p, d, c) =>
{
    if (t is not null)
        Ryzenadj.SetTdp(t.Value);
    
    if (e is not null)
        Sysfs.SetEnergyPerformancePreference(e);

    var coreSpecifications = (p ?? "")
        .Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    var cores = new List<int>();

    foreach (var spec in coreSpecifications)
    {
        var coreRange = spec
            .Split('-', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToArray();
        
        cores.AddRange(coreRange.Length == 2 ? Enumerable.Range(coreRange[0], coreRange[1] - coreRange[0] + 1) : coreRange);
    }
    
    if (p is not null)
        Sysfs.SetCpuCorePower(cores, false);
    
    // Drop privileges
    Libc.SetEffectiveUserId(Libc.GetUserId());
    
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

    Process
        .Start(
            "systemd-inhibit",
            $"""--what=idle:sleep --who=gamectl --why="Running game" -- {commandToExecute}""")
        .WaitForExit();

    if (m is not null && Configuration.DefaultMode is not null)
        DisplayMode.SetDisplayMode(Configuration.DefaultMode);
    
    // Regain privileges
    Libc.SetEffectiveUserId(0);
    
    if (t is not null && Configuration.DefaultTdp is not null)
        Ryzenadj.SetTdp(Configuration.DefaultTdp.Value);
    
    if (e is not null && Configuration.DefaultEpp is not null)
        Sysfs.SetEnergyPerformancePreference(Configuration.DefaultEpp);
    
    if (p is not null)
        Sysfs.SetCpuCorePower(cores, true);
}, eOption, tOption, gOption, mOption, pOption, dOption, cArgument);

await rootCommand.InvokeAsync(args);