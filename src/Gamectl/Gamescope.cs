namespace Gamectl;

public static class Gamescope
{
    public static string GetGamescopeCommandLine(int fps, string command)
    {
        if (Configuration.GamescopeArguments is null)
            return command;

        if (Drm.GetCards().Length > 1 && !Configuration.GamescopeOnExternalGpu)
            return command;
        
        return $"gamescope {Configuration.GamescopeArguments} -r {fps} -- {command}";
    }
}