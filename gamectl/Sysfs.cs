namespace gamectl;

public static class Sysfs
{
    public static void SetEnergyPerformancePreference(string epp)
    {
        var policies = Directory.GetDirectories("/sys/devices/system/cpu/cpufreq");
        
        foreach (var policy in policies)
            File.WriteAllText(Path.Join(policy, "energy_performance_preference"), epp);
    }
}