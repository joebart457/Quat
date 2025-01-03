
using System.Runtime.InteropServices;


namespace QuatLanguage.Interpreter.Memory;


public class GlobalMemoryManager : IMemoryManager
{
    public static GlobalMemoryManager Instance = new GlobalMemoryManager();
    public static GlobalMemoryManager CreateDetachedInstance() => new GlobalMemoryManager();

    private HashSet<IntPtr> _trackedPointers = new();
    public IntPtr AllocateNativeIntegerSize()
    {
        return AllocateAndTrack(IntPtr.Size);
    }

    public IntPtr AllocateString(string str)
    {
        return TrackMemory(Marshal.StringToHGlobalAnsi(str));
    }

    public nint AllocateStruct<T>(T tStruct) where T : struct
    {
        var size = Marshal.SizeOf(tStruct);
        var ptr = AllocateAndTrack(size);
        Marshal.StructureToPtr(tStruct, ptr, false);
        return ptr;
    }

    public IntPtr AllocateAndTrack(nint bytes)
    {
        return TrackMemory(Marshal.AllocHGlobal(bytes));
    }


    public IntPtr TrackMemory(IntPtr ptr)
    {
        _trackedPointers.Add(ptr);
        return ptr;
    }

    public bool FreeMemory(IntPtr ptr)
    {
        if (_trackedPointers.Remove(ptr))
        {
            Marshal.FreeHGlobal(ptr);
            return true;
        }
        return false;
    }

    public void ForceFree(IntPtr ptr)
    {
        Marshal.FreeHGlobal(ptr);
    }


    public void FreeAll()
    {
        foreach (var ptr in _trackedPointers)
        {
            FreeMemory(ptr);
        }
        _trackedPointers = new();
    }

    public byte ReadByte(IntPtr ptr)
    {
        return Marshal.ReadByte(ptr);
    }

    public nint ReadIntPtr(nint ptr)
    {
        return Marshal.ReadIntPtr(ptr);
    }

    public string? ReadAsString(IntPtr ptr)
    {
        return Marshal.PtrToStringAnsi(ptr);
    }

    public T ReadAsStruct<T>(IntPtr ptr) where T : struct
    {
        return Marshal.PtrToStructure<T>(ptr);
    }

    public void WriteIntPtr(nint ptr, nint valueToWrite)
    {
        Marshal.WriteIntPtr(ptr, valueToWrite);
    }

    public void WriteByte(nint ptr, byte valueToWrite)
    {
        Marshal.WriteByte(ptr, valueToWrite);
    }

}