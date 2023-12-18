using System.Runtime.InteropServices;

namespace gamectl;

public static class Libc
{
    [DllImport("libc", EntryPoint = "getuid")]
    public static extern int GetUserId();
    [DllImport("libc", EntryPoint = "geteuid")]
    public static extern int GetEffectiveUserId();
    [DllImport("libc", EntryPoint = "setuid")]
    public static extern int SetUserId(int userId);
    
    [DllImport("libc", EntryPoint = "getpwuid")]
    private static extern IntPtr GetPasswordFileEntryByUserId(int userId);

    public static string GetUserNameByUserId(int userId)
    {
        var ptr = GetPasswordFileEntryByUserId(userId);
        var passwdStruct = Marshal.PtrToStructure<passwd>(ptr);

        return passwdStruct.pw_name;
    }
}

public struct passwd
{
    [MarshalAs(UnmanagedType.LPStr)]
    public string pw_name;
    [MarshalAs(UnmanagedType.LPStr)]
    public string pw_passwd;
    public int pw_uid;
    public int pw_gid;
    [MarshalAs(UnmanagedType.LPStr)]
    public string pw_gecos;
    [MarshalAs(UnmanagedType.LPStr)]
    public string pw_dir;
    [MarshalAs(UnmanagedType.LPStr)]
    public string pw_shell;
}