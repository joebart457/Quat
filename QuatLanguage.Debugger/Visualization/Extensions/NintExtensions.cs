using QuatLanguage.Debugger.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using static QuatLanguage.Debugger.Visualization.Natives.NativeFunctions;

namespace QuatLanguage.Debugger.Visualization.Extensions;
internal static class NintExtensions
{
    public static string? GetStringValueOrNull(this nint nativeInt, DebuggableContext? context, int maxLen)
    {
        try
        {
            if (nativeInt == nint.Zero) return null;
            var maxLength = GetMaxStringLength(nativeInt, maxLen);
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
