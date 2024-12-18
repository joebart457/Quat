using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QuatLanguage.Debugger.Visualization.Natives.NativeFunctions;

namespace QuatLanguage.Debugger.Visualization.Services;

public static class ConsoleService
{

    private static IntPtr _stdHndl;
    private static IntPtr _curHndl;

    public static bool Initialize()
    {
        var newConsole = ConsoleFunctions.AllocConsole();
        _stdHndl = ConsoleFunctions.GetStdHandle(ConsoleConstants.STD_OUTPUT_HANDLE);
        _curHndl = ConsoleFunctions.CreateConsoleScreenBuffer(ConsoleConstants.GENERIC_READ | ConsoleConstants.GENERIC_WRITE, 0x00000001, IntPtr.Zero, 1, IntPtr.Zero);
        if (_curHndl == IntPtr.Zero) return false;
        return Activate();
    }

    public static bool Activate()
    {
        ConsoleFunctions.GetConsoleCursorInfo(_stdHndl, out var lpCursorInfo);
        lpCursorInfo.Visible = false;
        ConsoleFunctions.SetConsoleCursorInfo(_curHndl, ref lpCursorInfo);
        return ConsoleFunctions.SetConsoleActiveScreenBuffer(_curHndl);
    }

    public static void Cleanup()
    {
        // TODO close _curHndl
        ConsoleFunctions.SetConsoleActiveScreenBuffer(_stdHndl);
        ConsoleFunctions.CloseHandle(_curHndl);
        Console.CursorVisible = true;
    }
}