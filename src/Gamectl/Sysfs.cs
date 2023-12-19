namespace Gamectl;

public static class Sysfs
{
    public static void SetEnergyPerformancePreference(string epp)
    {
        var policies = Directory.GetDirectories("/sys/devices/system/cpu/cpufreq", "policy*");
        
        foreach (var policy in policies)
            File.WriteAllText(Path.Join(policy, "energy_performance_preference"), epp);
    }

    public static void SetCpuCorePower(IEnumerable<int> cores, bool online)
    {
        foreach (var core in cores)
        {
            var path = $"/sys/devices/system/cpu/cpu{core}/online";
            
            if (File.Exists(path))
                File.WriteAllText(path, online ? "1" : "0");
        }
    }
}