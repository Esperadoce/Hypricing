using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Hyprland.Configuration.Native;



public static class HyprlangWrapper
{
    // Shared parse context to hold the latest parse result from the callback invocation no MULTITHREADING SAFE
    public static ParseContext CurrentParseContext { get; } = new ParseContext();
    private const string LibraryName = "hyprlangwrap"; 
    // resolves to libhyprlangwrap.so on Linux
    // Signature must match C: void (*parse_cb_t)(const parse_result_t* r, void* user_data)
    // Use cdecl on Linux.
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static unsafe void OnParse(ParseResult* parseResult, void* userData)
    {
        if (parseResult == null) return;

        // Copy immediately: message points into out_buf and is only valid during callback.
        var msg = parseResult->message == null || parseResult->messageSize == 0
            ? string.Empty
            : Encoding.UTF8.GetString(parseResult->message, checked((int)parseResult->messageSize));

        bool isError = parseResult->error != 0;
        CurrentParseContext.Message = msg;
        CurrentParseContext.IsError = isError;
    }
    
    [DllImport("hyprlangwrap", EntryPoint = "hypr_config_parse_text", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe void HyprConfigParseText(
        byte* utf8Text,
        delegate* unmanaged[Cdecl]<ParseResult*, void*, void> callback,
        void* userData);
    
    public static void ParseText(string configText)
    {
        ArgumentNullException.ThrowIfNull(configText);

        // Prepare input text as UTF-8
        var utf8Bytes = Encoding.UTF8.GetBytes(configText);
        unsafe
        {
            fixed (byte* pUtf8Text = utf8Bytes)
            {
                // Call the native function with the callback
                HyprConfigParseText(
                    pUtf8Text,
                    &OnParse,
                    null);
            }
        }
    }
    
}
