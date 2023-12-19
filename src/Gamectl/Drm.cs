namespace Gamectl;

public static class Drm
{
    public static string[] GetCards()
    {
        return Directory.GetDirectories("/sys/class/drm", "card?");
    }
}