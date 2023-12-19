namespace Gamectl;

public static class Configuration
{
    private static readonly string[] ConfigurationPaths = new[]
    {
        "/etc/gamectl.conf", "~/.config/gamectl.conf"
    };

    public static int? DefaultTdp { get; private set; }
    public static string? DefaultEpp { get; private set; }
    public static string? DefaultMode { get; private set; }
    public static bool DisplayModeOnExternalGpu { get; private set; }
    public static bool GamescopeOnExternalGpu { get; private set; }
    
    public static void LoadConfiguration()
    {
        foreach (var path in ConfigurationPaths)
        {
            if (!File.Exists(path))
                continue;

            var lines = File.ReadAllLines(path).Where(l => !string.IsNullOrWhiteSpace(l) && l.Contains('='));

            foreach (var line in lines)
            {
                var parts = line.Split("=", 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                SetProperties(parts[0], parts[1]);
            }
        }
    }

    private static void SetProperties(string key, string value)
    {
        switch (key)
        {
            case "default_tdp": DefaultTdp = int.Parse(value); break;
            case "default_epp": DefaultEpp = value; break;
            case "default_mode": DefaultMode = value; break;
            
            case "display_mode_on_egpu": DisplayModeOnExternalGpu = value == "1"; break;
            case "gamescope_on_egpu": GamescopeOnExternalGpu = value == "1"; break;
        }
    }
}