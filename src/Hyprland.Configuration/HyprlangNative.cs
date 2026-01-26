using System.Runtime.InteropServices;

namespace Hyprland.Configuration;



public static class HyprlangNative
{
    private const string LibraryName = "hyprlangwrap"; 
    // resolves to libhyprlangwrap.so on Linux

    [StructLayout(LayoutKind.Sequential)]
    public struct HyprResult
    {
        public int Ok;
        public IntPtr Error; // char*
    }

    [DllImport(LibraryName, EntryPoint = "hypr_config_parse_text", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr HyprConfigParseText(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string utf8Text,
        out HyprResult result
    );

    [DllImport(LibraryName, EntryPoint = "hypr_config_parse_file", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr HyprConfigParseFile(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string utf8Path,
        out HyprResult result
    );

    [DllImport(LibraryName, EntryPoint = "hypr_config_get_diagnostics_json", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr HyprConfigGetDiagnosticsJson(IntPtr handle);

    [DllImport(LibraryName, EntryPoint = "hypr_config_destroy", CallingConvention = CallingConvention.Cdecl)]
    public static extern void HyprConfigDestroy(IntPtr handle);

    [DllImport(LibraryName, EntryPoint = "hypr_free_string", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void HyprFreeString(IntPtr str);


    public static string? PtrToUtf8AndFree(IntPtr ptr)
    {
        if (ptr == IntPtr.Zero)
            return null;

        try
        {
            return Marshal.PtrToStringUTF8(ptr);
        }
        finally
        {
            HyprFreeString(ptr);
        }
    }
}
