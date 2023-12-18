namespace Gamectl;

public static class Drm
{
    public static string[] GetCards()
    {
        return Directory.EnumerateDirectories("/sys/class/drm", "card?").ToArray();
    }
}