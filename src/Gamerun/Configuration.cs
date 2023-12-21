using Tomlyn;

namespace Gamerun;

public class Configuration
{
    private static readonly string[] ConfigurationPaths = {
        $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.config/gamerun.conf",
        "/etc/gamerun.conf"
    };

    private static Configuration? _values;

    public static Configuration Values => _values ??= ReadConfiguration();
    
    public int? DefaultTdp { get; set; }
    public string? DefaultEpp { get; set; }
    public string? DefaultDisplayMode { get; set; }
    public string? DefaultParkedCores { get; set; }
    public int? ExitTdp { get; set; }
    public string? ExitEpp { get; set; }
    public string? ExitDisplayMode { get; set; }
    public string? ExitParkedCores { get; set; }
    public string? GamescopeArguments { get; set; }
    public bool DisplayModeOnExternalGpu { get; set; }
    public bool GamescopeOnExternalGpu { get; set; }

    private static Configuration ReadConfiguration()
    {
        foreach (var path in ConfigurationPaths)
        {
            if (!File.Exists(path))
                continue;
            
            return Toml.ToModel<Configuration>(File.ReadAllText(path));
        }

        return new();
    }
}