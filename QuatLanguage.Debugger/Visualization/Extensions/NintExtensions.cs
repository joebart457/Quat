using QuatLanguage.Debugger.Context;
using System.Runtime.InteropServices;
using static QuatLanguage.Debugger.Visualization.Natives.NativeFunctions;

namespace QuatLanguage.Debugger.Visualization.Extensions;
internal static class NintExtensions
{
    public static string? GetStringValueOrNull(this nint nativeInt, DebuggableContext? context, int maxLen)
    {
        try
        {
            if (nativeInt == nint.Zero) return null;
            var maxLength = GetMaxStringLength(nativeInt, maxLen > 0? maxLen: 10000);
            if (maxLen == 0) maxLen = maxLength;
            if (maxLength == 0) return null;
            return Marshal.PtrToStringAnsi(nativeInt, Math.Min(maxLen, maxLength));
        }
        catch (AccessViolationException) { return null; }
    }

    public static int GetMaxStringLength(nint ptr, int max)
    {
        if (ConsoleFunctions.IsBadStringPtrA(ptr, 1)) return 0;
        int size = 1;
        while (size < max && !ConsoleFunctions.IsBadStringPtrA(ptr, (uint)size))
        {
            size++;
        }
        return size;
    }
}
