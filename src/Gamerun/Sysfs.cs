namespace Gamerun;

public static class Sysfs
{
    public static void SetEnergyPerformancePreference(string epp)
    {
        var policies = Directory.GetDirectories("/sys/devices/system/cpu/cpufreq", "policy*");
        
        foreach (var policy in policies)
            File.WriteAllText(Path.Join(policy, "energy_performance_preference"), epp);
    }

    public static IEnumerable<int> GetCpuCores()
    {
        return Directory
            .GetDirectories("/sys/devices/system/cpu", "cpu*")
            .Select(Path.GetFileName)
            .Select(d => int.TryParse(d![3..], out var id) ? id : (int?)null)
            .Where(d => d is not null)
            .Cast<int>();
    }
    
    public static void SetCpuCoresPower(IEnumerable<int> cores, bool online)
    {
        foreach (var core in cores)
        {
            var path = $"/sys/devices/system/cpu/cpu{core}/online";
            
            if (File.Exists(path))
                File.WriteAllText(path, online ? "1" : "0");
        }
    }

    public static bool GetBatteryChargerConnected()
    {
        foreach (var acPath in Directory.EnumerateDirectories("/sys/class/power_supply", "*AC*"))
        {
            var onlinePath = Path.Join(acPath, "online");

            if (File.Exists(onlinePath))
                return File.ReadAllText(onlinePath).TrimEnd() == "1";
        }

        return false;
    }
    
    public static string[] GetDrmCards()
    {
        return Directory.GetDirectories("/sys/class/drm", "card?");
    }
}