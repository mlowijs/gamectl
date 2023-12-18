using System.Globalization;
using System.Text.RegularExpressions;
using gamectl.Kde;

namespace gamectl;

public static class DisplayMode
{
    private static readonly Regex ModeRegex = new(@"(\d+)x(\d+)@(\d+)", RegexOptions.Compiled);
    
    public static void SetDisplayMode(string mode)
    {
        var match = ModeRegex.Match(mode);

        var (width, height, refreshRate) = (
            int.Parse(match.Groups[1].Value),
            int.Parse(match.Groups[2].Value), 
            decimal.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture));
        
        switch (Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP"))
        {
            case "KDE":
                var displayConfiguration = KscreenDoctor.GetDisplayConfiguration();
                
                return;
            
            default:
                return;
        }
    }
}