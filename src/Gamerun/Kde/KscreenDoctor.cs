using System.Diagnostics;
using System.Text.Json;

namespace Gamerun.Kde;

public static class KscreenDoctor
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public static KscreenDoctorResult GetDisplayConfiguration()
    {
        var process = Process.Start(new ProcessStartInfo("/usr/bin/kscreen-doctor", "-j")
        {
            RedirectStandardOutput = true
        });
        process!.WaitForExit();

        var jsonString = process.StandardOutput.ReadToEnd();
        return JsonSerializer.Deserialize<KscreenDoctorResult>(jsonString, SerializerOptions)!;
    }

    public static void SetDisplayMode(string output, string mode)
    {
        Process.Start("/usr/bin/kscreen-doctor", $"output.{output}.mode.{mode}").WaitForExit();
    }
}