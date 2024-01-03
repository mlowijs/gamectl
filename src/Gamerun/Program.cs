using System.CommandLine;
using System.Diagnostics;
using Gamerun;

if (Libc.GetEffectiveUserId() != 0 && !Debugger.IsAttached)
{
    Console.WriteLine("This application must be run suid root");
    return;
}

var rootCommand = CommandLine.CreateRootCommand((e, g, m, p, t, c) =>
{
    if (c.Length == 0)
    {
        Console.WriteLine("Command argument is required.");
        return;
    }

    var tdp = Sysfs.IsBatteryChargerConnected()
        ? Configuration.Values.DefaultAcTdp ?? t
        : t ?? Configuration.Values.DefaultDcTdp;
        
    if (tdp is not null)
        Ryzenadj.SetTdp(tdp.Value);
    
    var epp = e ?? Configuration.Values.DefaultEpp;
    if (epp is not null)
        Sysfs.SetEnergyPerformancePreference(epp);
    
    Sysfs.SetCpuCoresPower(Functions.ParseCoresSpecification(p ?? Configuration.Values.DefaultParkedCores), false);
    
    // Drop privileges
    Libc.SetEffectiveUserId(Libc.GetUserId());

    var mode = m ?? Configuration.Values.DefaultDisplayMode;
    if (mode is not null)
        DisplayMode.SetDisplayMode(mode);
    
    for (var i = 0; i < c.Length; i++)
    {
        if (c[i].Contains(' '))
            c[i] = $"\"{c[i]}\"";
    }
    
    var commandString = string.Join(' ', c);
    var commandToExecute = g ? Gamescope.GetGamescopeCommandLine(commandString) : commandString;

    Process
        .Start(
            "systemd-inhibit",
            $"""--what=idle:sleep --who=gamerun --why="Running game" -- {commandToExecute}""")
        .WaitForExit();

    if (Configuration.Values.ExitDisplayMode is not null)
        DisplayMode.SetDisplayMode(Configuration.Values.ExitDisplayMode);
    
    // Regain privileges
    Libc.SetEffectiveUserId(0);
    
    Sysfs.SetCpuCoresPower(Functions.ParseCoresSpecification(Configuration.Values.ExitParkedCores), true);
    
    if (Configuration.Values.ExitEpp is not null)
        Sysfs.SetEnergyPerformancePreference(Configuration.Values.ExitEpp);
    
    if (Configuration.Values.ExitTdp is not null)
        Ryzenadj.SetTdp(Configuration.Values.ExitTdp.Value);
});

await rootCommand.InvokeAsync(args);