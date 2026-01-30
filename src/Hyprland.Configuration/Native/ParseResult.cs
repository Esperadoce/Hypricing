using System.Runtime.InteropServices;

namespace Hyprland.Configuration.Native;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct ParseResult
{
    public byte error;          // uint8_t
    public byte* message;       // char*
    public uint messageSize;    // uint32_t
}