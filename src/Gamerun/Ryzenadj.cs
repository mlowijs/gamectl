using System.Diagnostics;

namespace Gamerun;

public static class Ryzenadj
{
    public static void SetTdp(int tdp)
    {
        var tdpInMw = tdp * 1000;
        
        Process
            .Start("/usr/bin/ryzenadj", $"-a {tdpInMw} -b {tdpInMw} -c {tdpInMw}")
            .WaitForExit();
    }
}