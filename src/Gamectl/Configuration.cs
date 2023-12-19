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
    public static string? GamescopeArguments { get; private set; }
    public static bool DisplayModeOnExternalGpu { get; private set; }
    public static bool GamescopeOnExternalGpu { get; private set; }
    
    public static void LoadConfiguration()
    {
        foreach (var path in ConfigurationPaths)
        {
            if (!File.Exists(path))
                continue;

            var lines = File.ReadAllLines(path);

            foreach (var line in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
            {
                var entryAndComment = line.Split("#", 2, StringSplitOptions.TrimEntries);
                var entry = entryAndComment[0];
                
                if (string.IsNullOrWhiteSpace(entry) || !entry.Contains('='))
                    continue;
                
                var keyValue = entry.Split("=", 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                SetProperties(keyValue[0], keyValue[1]);
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
            case "gamescope_args": GamescopeArguments = value; break;
            case "display_mode_on_egpu": DisplayModeOnExternalGpu = value == "1"; break;
            case "gamescope_on_egpu": GamescopeOnExternalGpu = value == "1"; break;
        }
    }
}