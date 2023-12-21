using System.Runtime.InteropServices;

namespace Gamerun;

public static class Libc
{
    [DllImport("libc", EntryPoint = "getuid")]
    public static extern int GetUserId();
    [DllImport("libc", EntryPoint = "geteuid")]
    public static extern int GetEffectiveUserId();

    [DllImport("libc", EntryPoint = "seteuid")]
    public static extern int SetEffectiveUserId(int userId);
}