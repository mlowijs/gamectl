using System.Diagnostics;

namespace gamectl;

public class Gamescope
{
    public static string GetGamescopeCommandLine(int fps, string command)
    {
        return $"gamescope -f -F fsr -h 720 -H 1080 -r {fps} -- {command}";
    }
}